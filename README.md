# InstanTTS
A program designed to allow people to use TTS through voice chat in conjunction with VB Virtual Audio Cable.

Uses [NAudio](https://github.com/naudio/NAudio) by Mark Heath.

## Usage
Download the latest stable release, unzip the contents into a folder and run the executable.

If you want to use a development branch, clone the repository, install NAudio via NuGet, and build the executable from source.

In order to use the software *as intended* and not just to play text-to-speech for yourself, install [VB Virtual Audio Cable](https://vb-audio.com/Cable/) or any other virtual audio device of choice. Set your output device (under the **Settings** tab) to `CABLE Input` or equivalent, then set the input device for the target program (e.g. Discord voice chat) to `CABLE Output` or equivalent.

**Disclaimer: I am not accountable for damage of any kind caused by the misuse of this software. This includes, but is not limited to:**
- Users speaking the phrase "John Madden" on repeat in voice chats
- Users mic spamming at the lowest possible speech rate
- Emotional trauma resulting from the emotionless narration of certain copypastas
- Anyone using BonziBuddy as their installed voice

## Interface
The interface for InstanTTS consists of two tabs, **Speech** and **Settings**.

### Speech
The **Speech** tab contains functionality relevant to actually using the application.

Your *speech history* occupies most of this screen. Whenever you send a message, it will be logged to this component alongside the voice, audio device, rate and volume used.

On the right side of the screen, *voice*, *rate* and *volume* controls are available. 
- *Voice* allows you to select one of the TTS voices installed on your system. This will be used next time text is queued.
- *Rate* adjusts the speech rate.
- *Volume* adjusts the base volume of the audio.

Below this, queued text is displayed. This area looks terrible and needs work.

At the bottom of the TTS queue, you'll find the *pause* and *skip* buttons. These should speak for themselves, but for clarity's sake:
- *Pause* pauses the current TTS clip.
- *Skip* skips the current clip and goes straight to playing the next one.

### Settings
The **Settings** tab currently only contains one thing in the main branch - the ability to set your *Output Devices*.

With the version available in the ```hotkeys``` branch, you may also configure custom hotkeys.
1. Click the button next to `"New hotkey:"`.
2. Press the key you wish to use. Modifier keys (Shift, Ctrl, Alt, and Windows) are taken into account but cannot be used independently.
3. Enter the text to speak when this hotkey is pressed.
4. Click **Add Hotkey**.

Hotkeys use the current speech controls for voice, rate and volume.

For convenience, these hotkeys will **not** play while the application has focus.

## Planned Features
"Wow, basic functionality that still hasn't been implemented!" - you, probably

no mean to me :( i'm a broke college student who wrote this in a week for the sake of resume padding then realized it was actually a decent project idea

- Ability to save and load settings profiles, including default settings.
- ~~Ability to remove hotkeys. Yes, really.~~ **done**
- ~~Ability to repeat any line from your speech history with one click.~~ **done**
    - add another button to repeat with current settings ("repeat text")
- Write an observable dictionary implementation so hotkeys update immediately.
- Add the ability to edit hotkeys after creation.
- An overlay so you don't have to tab out to type.
- Program hotkeys (repeat, etc.).
