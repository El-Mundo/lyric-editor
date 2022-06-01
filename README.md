# lyric-editor
This Android APP helps build IMV4s, a data structure for lyric-based music visualization.


![image](https://github.com/El-Mundo/lyric-editor/blob/master/snapshot.png)

# Download

English version:

https://github.com/El-Mundo/lyric-editor/releases/download/release/IMV4_Editor-Pro.apk

Chinese version:

https://github.com/El-Mundo/lyric-editor/releases/download/first-release/IMV4_Editor-Pro.apk

# User Manual

https://github.com/El-Mundo/lyric-editor/releases/download/release/IMV4_Editor_Guidance_Mannual.pdf

# IMV4 Player

![image](https://github.com/El-Mundo/lyric-editor/blob/master/player.png)

The player is specially designed to play this file type.
It by default plays the video attached the music but will generate a werid animation if the attached media is not a video.

Online Archive-Accessible Version:

https://github.com/El-Mundo/lyric-editor/releases/download/release/LyricInteractions-OnlineArchive.apk

No Internet Access (Older) Version:

https://github.com/El-Mundo/lyric-editor/releases/download/first-release/LyricInteractions.apk

# IMV4 Data Structure

      -Song

        -Sentence
    
          -Chunk
    
            -(Alternative) Unit
      
              -MIDI Note
              
      -Media Flag
      
        -Encoded Media (base64)
        
# Known Issue
The PC edition cannot open MP3 files due to a native BUG of Unity Engine.
