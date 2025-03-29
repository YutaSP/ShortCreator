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

# Example usage:
delete_files_from_directory('Vids/')