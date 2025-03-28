from gtts import gTTS
from pydub import AudioSegment
import ffmpeg
import srt
from moviepy import VideoFileClip, CompositeVideoClip, TextClip, AudioFileClip
import pyodbc
import os

def delete_files_from_directory(directory):
    # Loop through all the files in the given directory
    for filename in os.listdir(directory):
        file_path = os.path.join(directory, filename)
        
        # Check if it's a file and not a directory
        if os.path.isfile(file_path):
            os.remove(file_path)
            print(f"Deleted {file_path}")
        else:
            print(f"Skipping directory {file_path}")

# Function to generate speech
def generate_speech(text, language, output_filename, speed_factor=1.075):
    # Generate speech using gTTS
    tts = gTTS(text=text, lang=language, slow=False)
    
    # Save the speech to a temporary file
    temp_filename = "temp_audio.mp3"
    tts.save(temp_filename)
    print(f"Speech saved to {temp_filename}")
    
    # Load the audio with pydub
    audio = AudioSegment.from_mp3(temp_filename)
    
    # Speed up the audio by the given factor (1.25 times faster)
    new_audio = audio.speedup(playback_speed=speed_factor)
    
    # Export the speed-adjusted audio to the final output filename
    new_audio.export(output_filename, format="mp3")
    print(f"Speech with adjusted speed saved to {output_filename}")
    
    # Optionally delete the temporary file if needed
    os.remove(temp_filename)

# Function to create subtitles
def generate_subtitles(text, audio_filename, output_filename):
    sentences = text.split('. ')  # Split text into sentences for subtitles

    # Get the total duration of the audio
    audio_duration = get_audio_duration(audio_filename)

    # Calculate the duration each subtitle should be displayed
    sentence_duration = audio_duration / len(sentences)

    subtitles = []
    start_time = 0  # Start from 0 seconds

    for idx, sentence in enumerate(sentences):
        end_time = start_time + sentence_duration  # Adjust based on total duration / sentence count
        subtitle = srt.Subtitle(index=idx+1,
                                start=srt.timedelta(seconds=start_time),
                                end=srt.timedelta(seconds=end_time),
                                content=sentence)
        subtitles.append(subtitle)
        start_time = end_time

    with open(output_filename, 'w') as f:
        f.write(srt.compose(subtitles))
    print(f"Subtitles saved to {output_filename}")

# Function to overlay subtitles on video
def overlay_subtitles_on_video(video_filename, subtitle_filename, audio_filename, output_filename):
    # Load the video
    video = VideoFileClip(video_filename)
    
    # Load the audio (ensure it's a valid audio file like .mp3 or .wav)
    audio = AudioFileClip(audio_filename)  # Use AudioFileClip to load the audio file

    video = video.subclipped(0, audio.duration)
    # Read the subtitle file
    with open(subtitle_filename, 'r') as f:
        subtitles = f.read().split('\n\n')
    
    # Create subtitle clips
    subtitle_clips = []
    for subtitle in subtitles:
        lines = subtitle.split('\n')
        
        # Ensure there are at least 3 lines (index, timestamp, text)
        if len(lines) < 3:
            print(f"Skipping malformed subtitle block: {subtitle}")
            continue
        
        index = lines[0]
        times = lines[1]
        textLine = lines[2]
        
        # Convert start and end times to seconds
        try:
            start_time, end_time = times.split(' --> ')
            start_seconds = convert_time_to_seconds(start_time)
            end_seconds = convert_time_to_seconds(end_time)
        except ValueError as e:
            print(f"Skipping subtitle with invalid time format: {times}")
            continue
        
        # Debug: Check parameters before creating TextClip
        print(f"Creating subtitle clip for: {textLine}, {start_seconds} to {end_seconds} seconds.")
        
        try:
            # Remove the font argument to use the default font
            font_path = "C:/Windows/Fonts/arial.ttf"  # Path to Arial font file on Windows
            subtitle_clip = TextClip(
                font=font_path,
                text = textLine, 
                font_size=50, 
                color='white', 
                method="caption",  # Wrap text properly
                size=(int(video.w * 0.8), None)             
            )
            subtitle_clip = subtitle_clip.with_position(("center", "center"))  # Position at the bottom center
            subtitle_clip = subtitle_clip.with_duration(end_seconds - start_seconds)
            subtitle_clip = subtitle_clip.with_start(start_seconds)
            
            subtitle_clips.append(subtitle_clip)
        except Exception as e:
            print(f"Error creating subtitle: {e}")
            continue
    
    # Combine the video with subtitle clips
    final_video = CompositeVideoClip([video] + subtitle_clips)
    
    # Set the audio for the final video
    final_video = final_video.with_audio(audio)
    
    # Write the final output video to a file
    final_video.write_videofile(output_filename, fps=24, codec="libx264", audio=True)

# Function to merge audio and video
def merge_audio_video(video_filename, audio_filename, output_filename):
    try:
        # Define the input for video
        video_input = ffmpeg.input(video_filename)
        
        # Define the input for audio
        audio_input = ffmpeg.input(audio_filename)
        video_input_fast = video_input.filter('setpts', '0.8*PTS')
        audio_input_fast = audio_input.filter('atempo', 1.25)
        # Combine video and audio inputs and set output parameters
        ffmpeg.output(video_input_fast, audio_input_fast, output_filename, vcodec='libx264', acodec='aac').run()
        
        print(f"Video and audio merged into {output_filename}")
    except Exception as e:
        print(f"Error while merging video and audio: {e}")

# Function to query the database
def query_top_1_record(connection_string, table_name):
    conn = pyodbc.connect(connection_string)
    cursor = conn.cursor()

    query = f"SELECT TOP 1 * FROM {table_name}"
    cursor.execute(query)
    result = cursor.fetchone()

    if result:
        return result
    else:
        return None

# Function to convert time from SRT format to seconds
def convert_time_to_seconds(time_str):
    hours, minutes, seconds = time_str.split(':')
    seconds, milliseconds = seconds.split(',')
    return int(hours) * 3600 + int(minutes) * 60 + int(seconds) + int(milliseconds) / 1000

# Function to get the duration of the audio
def get_audio_duration(audio_filename):
    audio = AudioFileClip(audio_filename)
    return audio.duration

# Main function
def main():
    connection_string = 'DRIVER={ODBC Driver 17 for SQL Server};SERVER=localhost,1433;DATABASE=Short_Maker_Prod;UID=sa;PWD=!Test123'
    table_name = 'Reddit_Stories'
    result = query_top_1_record(connection_string, table_name)
    
    if not result:
        print("No record found!")
        return

    vidId = result[0]
    vidTitle = result[1]
    text = result[2]
    language = 'en'
    
    # File paths
    output_audio = f"Temp/{vidId}_output.mp3"
    subtitle_file = f"Temp/{vidId}_story.srt"
    background_video = 'Background/sample_comp.mp4'
    final_video = f"Temp/{vidId}_final_video.mp4"
    final_output = f"Vids/{vidTitle}_final_output.mp4"

    # Step 1: Generate speech
    generate_speech(text, language, output_audio)

    # Step 2: Generate subtitles
    generate_subtitles(text, output_audio, subtitle_file)

    # Step 3: Get audio duration and trim the video accordingly
    audio_duration = get_audio_duration(output_audio)

    # Load the video and trim it to match audio duration
    video = VideoFileClip(background_video)
    video = video.subclipped(0, audio_duration)

    # Step 4: Overlay subtitles on the video and add the audio
    overlay_subtitles_on_video(video.filename, subtitle_file, output_audio, final_video)

    # Step 5: Merge video and audio
    merge_audio_video(final_video, output_audio, final_output)
    delete_files_from_directory('Temp/')

# Run the main function
if __name__ == "__main__":
    main()
