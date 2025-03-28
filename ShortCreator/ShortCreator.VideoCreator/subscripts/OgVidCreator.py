import os
import subprocess

# Configuration
INPUT_DIR = "Samples"  # Change to your video directory
OUTPUT_DIR = "Vids"  # Converted video directory
FINAL_OUTPUT = "final_video.mp4"  # Merged output video
TARGET_DURATION = 60  # Total time (in seconds) to loop each video
RESOLUTION = "1080x1920"
FPS = 30

# Ensure output directory exists
os.makedirs(OUTPUT_DIR, exist_ok=True)

def get_video_files(directory):
    """Get all video files in the directory."""
    return [f for f in os.listdir(directory) if f.lower().endswith(('.mp4', '.mov', '.avi', '.mkv'))]

def convert_and_loop_video(input_file, output_file, duration, resolution, fps):
    """Convert video to the specified format and loop it until the required duration."""
    temp_output = output_file.replace(".mp4", "_temp.mp4")

    # Convert to standard format
    convert_cmd = [
        "ffmpeg", "-i", input_file,
        "-vf", f"scale={resolution},fps={fps}",
        "-c:v", "libx264", "-preset", "fast", "-crf", "23",
        "-c:a", "aac", "-b:a", "128k",
        temp_output
    ]
    subprocess.run(convert_cmd, stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True)

    # Get original video duration
    probe_cmd = [
        "ffprobe", "-i", temp_output, "-show_entries",
        "format=duration", "-v", "quiet", "-of", "csv=p=0"
    ]
    duration_output = subprocess.run(probe_cmd, stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True)
    original_duration = float(duration_output.stdout.strip())

    # Calculate loop count
    loop_count = max(1, int(duration / original_duration) + 1)

    # Loop video
    loop_cmd = [
        "ffmpeg", "-stream_loop", str(loop_count),
        "-i", temp_output, "-t", str(duration),
        "-c:v", "copy", "-c:a", "copy",
        output_file
    ]
    subprocess.run(loop_cmd, stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True)

    # Clean up temp file
    os.remove(temp_output)
    print(f"Processed: {output_file}")

def merge_videos(video_list, output_file):
    """Merge all processed videos into a single file."""
    list_file = "video_list.txt"

    # Create a text file listing all videos
    with open(list_file, "w") as f:
        for video in video_list:
            f.write(f"file '{video}'\n")

    # Merge using ffmpeg
    merge_cmd = [
        "ffmpeg", "-f", "concat", "-safe", "0",
        "-i", list_file, "-c", "copy", output_file
    ]
    subprocess.run(merge_cmd, stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True)

    print(f"Final merged video created: {output_file}")

def process_videos(input_dir, output_dir, duration, resolution, fps):
    """Process all videos in the directory and merge them."""
    videos = get_video_files(input_dir)
    if not videos:
        print("No videos found in the directory.")
        return

    converted_videos = []
    
    for video in videos:
        input_path = os.path.join(input_dir, video)
        output_path = os.path.join(output_dir, f"converted_{video}")
        convert_and_loop_video(input_path, output_path, duration, resolution, fps)
        converted_videos.append(output_path)

    # Merge all videos
    final_output_path = os.path.join(output_dir, FINAL_OUTPUT)
    merge_videos(converted_videos, final_output_path)

if __name__ == "__main__":
    process_videos(INPUT_DIR, OUTPUT_DIR, TARGET_DURATION, RESOLUTION, FPS)
    print("All videos processed and merged successfully!")
