# RAVEN Project
 
## 1. Introduction
(**We welcome any suggestions! Please provide them by emailing the first author, Xinyun Cao, at xinyunc@umich.edu directly.**)<br/>
This is a Unity project that showcases the RAVEN, a system that enables BLV users to issue queries and modification prompts to improve the runtime accessibility of 3D virtual scenes. The system is developed in Unity version 2021.3.8f1.

## 2. Project Structure
The following are the sub-folders in the project's Assets folder and their corresponding contents.
**_RAVEN_Util**: Contains prefabs and scripts that a developer might need to attach to the scene to incorporate RAVEN. <br />
**Materials**: Contains the Materials used in the RAVEN system and the Example Scene. <br />
**Packages**: Contains packages used in the project, including TextMeshPro, Text-to-speech, Windows Voice, and Web Socket packages. <br />
**Prefabs**: Contains prefabs that are used in the system and example scenes, including canvas and the loading icon. <br />
**Resources**: Contains the audio file for the torch fire sound used in the Example Scene. <br />
**Scenes**: Contain one scene, Example Scene, which showcases an example setup of RAVEN. <br />
**Scripts**: The scripts that cover the inner workings of RAVEN. User need not interact with this folder, only need to interact with _RAVEN_Util. <br />

## 3. Implementation Steps
To implement RAVEN, use the following steps:
### Step 1. Add Manager
Add the RAVEN_Managers prefab (in the _RAVEN_Util folder) into your scene. This will include all the inner logic of the system and the UI.

<img width="1880" height="1317" alt="A screenshot showing moving RAVEN_Manager prefab into a scene" src="https://github.com/user-attachments/assets/5ff8aeab-bc83-4b35-8eeb-8d344cedb09b" />

### Step 2. Indicate Player
Make sure your player is named “Player”. Add a TextDescription.cs to your Player object. Check the “Meta Obj” and “Is Player” booleans to be true.

<img width="803" height="425" alt="A screenshot showing the Meta Obj and Is Player booleans being checked" src="https://github.com/user-attachments/assets/87e9adf6-2ec1-4255-b0b0-6d907db96d3a" />

### Step 3. Tag Important Scene Objects
Now, for each important object in the scene, attach a TextDescription.cs file. Make sure the naming of the game object is semantic (not random gibberish). Add **optional** AdditionalDescription if you wish (these are any additional semantic, sound, etc. info you want to add). <br />
Make sure “Meta Obj” and “Is Player” tags are accurate.
- Meta Obj: Whether this object has a meaningful 3D location (not meta), or if it is abstract, like sunlight, ambient sound, etc (meta)
- Is Player: pretty self-explanatory

Note on "important objects":
- Important object: Things that are important to your (imaginary) scene/game experience for Blind and Low vision users. A key, a health potion, critical sound sources, etc.
- Less important objects: purely decorative objects, environmental elements less relevant to the experience.

### Step 4. Add API keys
The RAVEN system needs 2 API keys.
1. Under **RAVEN_Manager** in your scene, there is a child object named **GPTManagers**, which has a Component Script named **Custom GPT**. It has a field named Api Key. You need to provide your GPT API key in this field.

<img width="2559" height="1366" alt="A screenshot pointing out the Api Key field in the GPTManagers object" src="https://github.com/user-attachments/assets/b92eff51-8063-43f5-8325-08265a64c422" />

2. If you want the Text-to-speech component of RAVEN to work, you need to click on another child object of **RAVEN_Manager** named **TEXT_TO_SPEECH**. This object should have a Component Script **Text To Speech** that has a field named Api Key. You need to provide your Google Cloud API key in this field.

<img width="2559" height="1369" alt="A screenshot pointing out the Api Key field in the TEXT_TO_SPEECH object" src="https://github.com/user-attachments/assets/49c5a68f-145a-418b-a53d-2b3530d818fc" />

## 4. Acknowledgement
This system is built on top of GROMIT by Nicholas Nennings et. al., open-sourced here: https://github.com/NicholasJJ/GROMIT.
