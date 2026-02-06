# RAVEN: Realtime Accessibility in Virtual ENvironments for Blind and Low-vision People
[Xinyun Cao](https://xinyun-cao.github.io/), [Kexin Phyllis Ju](https://www.phyllisju.com/), [Chenglin Li](https://chenglinli.com/), [Venkatesh Potluri](https://venkateshpotluri.me/), [Dhruv Jain](https://web.eecs.umich.edu/~profdj/)

See our [preprint](https://doi.org/10.48550/arXiv.2510.06573) for more details. Full publication link coming soon!

## Introduction
This is the opensourced artifact of RAVEN: Realtime Accessibility in Virtual ENvironments for Blind and Low-Vision People. RAVEN is a system developed in Unity that enables BLV users to issue queries and modification prompts to improve the runtime accessibility of 3D scenes. This repo gives developers instructions to download a Unity starter project that has the full RAVEN setup. Developers can then modify the system or incorporate RAVEN into their own projects.

(**We welcome any questions, comments, or suggestions! Please provide them by emailing the first author, Xinyun Cao, at xinyunc@umich.edu directly.**)<br/>

## System Requirements
To run RAVEN, you need to have a Unity Editor downloaded. We recommend Unity 2021.3.8f1 for smoothest experience because this version is used during RAVEN's development, but later editor versions should also work. The editor can be downloaded here: https://unity.com/releases/editor/whats-new/2021.3.8f1.

## API Keys

You need two API keys for RAVEN:
1. a GPT API key: this is esential for the core generative functionality of RAVEN. RAVEN was evaluated using gpt-4o, but we welcome experimentation with other models. [GPT API site](https://openai.com/api/)
2. a Google Cloud API key: this is needed for the text-to-speech read back, which is an optional feature. [Google Cloud API site](https://cloud.google.com/apis)

See Implementation Steps->Step 4 for how to add API keys to the scene.

## Project Structure

Below is an overview of the subfolders contained in the project’s **Assets** directory and their purposes.
#### Contents
1. **_RAVEN_Util**  
   Contains prefabs and scripts that developers attach to a scene to integrate **RAVEN**.

2. **Materials**  
   Materials used by the RAVEN system and the Example Scene.

3. **Packages**  
   Third-party and Unity packages used in the project, including:
   - TextMeshPro  
   - Text-to-Speech  
   - Windows Voice  
   - WebSocket packages  

4. **Prefabs**  
   Prefabs used throughout the system and example scenes, such as:
   - UI canvas  
   - Loading icon  

5. **Resources**  
   Audio assets used in the Example Scene (e.g., torch fire sound).

6. **Scenes**  
   Contains a single scene:
   - **Example Scene** — demonstrates a reference setup of RAVEN.

7. **Scripts**  
   Core scripts that implement the internal functionality of RAVEN.

   **Script Categories**
   - **CSCompilation** – Runtime compilation of generated scripts  
   - **GPTScripts** – Interfaces with the GPT agent  
   - **InputSystem** – Handles UI and system input  
   - **OutputSystem** – Applies RAVEN output to the scene  
   - **SemanticSceneGraph** – Builds and manages the semantic scene graph  
   - **MenuScripts** – Implements the in-system helper menu  
   - **MiscScripts** – Miscellaneous utilities (UI helpers, player movement, etc.)

> **Note:**  
> Developers do **not** need to interact directly with the `Scripts` folder.  
> To use RAVEN, only the contents of `_RAVEN_Util` need to be added to a scene.


## Implementation Steps
To implement RAVEN, use the following steps:
### Step 1. Add Manager
Add the RAVEN_Managers prefab (in the _RAVEN_Util folder) into your scene. This will include all the inner logic of the system and the UI.

<img width="1880" height="1317" alt="A screenshot showing moving RAVEN_Manager prefab into a scene" src="https://github.com/user-attachments/assets/5ff8aeab-bc83-4b35-8eeb-8d344cedb09b" />

### Step 2. Indicate Player
Make sure your player is named “Player”. Add a TextDescription.cs to your Player object. Check the “Meta Obj” and “Is Player” booleans to be true.

<img width="803" height="425" alt="A screenshot showing the Meta Obj and Is Player booleans being checked" src="https://github.com/user-attachments/assets/87e9adf6-2ec1-4255-b0b0-6d907db96d3a" />

### Step 3. Tag Important Scene Objects
1. First, identify the important objects in the scene. Note on "important objects":
- Important object: Things that are important to your (imaginary) scene/game experience for Blind and Low vision users. A key, a health potion, critical sound sources, etc.
- Less important objects: purely decorative objects, environmental elements less relevant to the experience.

2. Now make sure the names of the "important" game objects are semantic (e.g., `health potion`, `enemy souldier #3`. The formating is flexible, but should **not** something like `casual_city_model_5781`).

3. Then, for each important object, add a `TextDescription` component. You can do so using `Add Component` -> `TextDescription` or drag and drop `TextDescription.cs` file onto the object's inspector view.

4. Lastly, fill out the variable fields in the `TextDescription` component. <br />
- Meta Obj: Whether this object has a meaningful 3D location (not meta), or if it is abstract, like sunlight, ambient sound, etc (meta)
- Is Player: whether this object is the player or not.
- **optional** AdditionalDescription: if you wish (these are any additional semantic, sound, etc. info you want to add).

### Step 4. Add API Keys
The RAVEN system needs 2 API keys.
1. The GPT API keys supports the generation of code and text for RAVEN system. Under **RAVEN_Manager** in your scene, there is a child object named **GPTManagers**, which has a Component Script named **Custom GPT**. It has a field named Api Key. You need to provide your GPT API key in this field.

<img width="2559" height="1366" alt="A screenshot pointing out the Api Key field in the GPTManagers object" src="https://github.com/user-attachments/assets/b92eff51-8063-43f5-8325-08265a64c422" />

2. If you want the optional Text-to-speech component to work, you need to click on another child object of **RAVEN_Manager** named **TEXT_TO_SPEECH**. This object should have a Component Script **Text To Speech** that has a field named Api Key. You need to provide your Google Cloud API key in this field.

<img width="2559" height="1369" alt="A screenshot pointing out the Api Key field in the TEXT_TO_SPEECH object" src="https://github.com/user-attachments/assets/49c5a68f-145a-418b-a53d-2b3530d818fc" />

### Step 5. Run the Scene and Use the System
Now your scene should be ready to run with RAVEN supported! Hit the Unity "Play" button. Then hit the "Enter" key. You should see a text box appear and hear the system voice "ready for prompt entry". You can then type your command, and hit "enter" key again to sent the command to the gpt agent. A spinning circle indicates that the system is loading. Once loading finishes, the system will compile the changes into the scene and voice out textual responses.

See here for a demo of the system in action in two mock scenarios:

https://github.com/user-attachments/assets/d98af928-dcbb-447b-8352-4d7972226ada

## Attribution
If this work inspires yours, consider citing us as follows:
<pre>
@inproceedings{10.1145/3772318.3791616,
    author = {Cao, Xinyun and Ju, Kexin Phyllis and Li, Chenglin and Potluri, Venkatesh and Jain, Dhruv},
    title = {RAVEN: Realtime Accessibility in Virtual ENvironments for Blind and Low-Vision People},
    year = {2026},
    isbn = {9798400722783},
    publisher = {Association for Computing Machinery},
    address = {New York, NY, USA},
    url = {https://doi.org/10.1145/3772318.3791616},
    doi = {10.1145/3772318.3791616},
    booktitle = {Proceedings of the 2026 CHI Conference on Human Factors in Computing Systems},
    location = {Barcelona, Spain},
    series = {CHI '26}
}
</pre>

## Acknowledgement
RAVEN extends GROMIT by Nicholas Jennings et. al., full paper linked here: https://doi.org/10.1145/3654777.3676358, artifact open-sourced here: https://github.com/NicholasJJ/GROMIT.
