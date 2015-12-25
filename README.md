### girltalkinabox

This is a WIP of a C# clone for http://static.echonest.com/girltalkinabox/

My goal is to get a copy of their interface running, then use my MIDI keyboard to control the song's playback.
E.g. every 5 keys on the keyboard correct to going -2...+2 beats after the current beat is finished playing.

It's kinda working, but since I'm feeding the audio data into the sound card one beat at a time, there's a noticable delay between them. I need to transform it from "play a beat, wait for it to end, play the next beat" to "keep a rolling buffer of the next beats we're going to play, fed into the WaveSource already" which sounds like not fun so I'm calling this a stopping point for now.