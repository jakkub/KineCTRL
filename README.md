kinectrl
========

A customizable 3D gestural interface for Windows.


## Introduction
**KineCTRL** is a Windows application that serves as a gestural interface for the desktop environment. Using KineCTRL on your computer, you can **define your own set of full-body 3D gestures**, and use these gestures to **control the operating system** or any other Windows application.

KineCTRL lets you create four types of gestures: **Body Gestures**, **Left Hand Gestures**, **Right Hand Gestures** and **Postures**. Each defined gesture can be assigned a key, or a keyboard shortcut, which will be executed in the desktop environment once the gesture is recognized by the application. Body, Left Hand and Right Hand gestures are **dynamic gestures** based on the motion of a certain part of the body and their purpose is to emulate a key press. Postures are **static gestures** based on your current body pose and they are used to emulate a key being held.

With KineCTRL, you can also control the mouse cursor using **hand movements**.

### Minimum Hardware Requirements
**CPU:** Dual-core 2.66 GHz
**RAM:** 4 GB
**Kinect for Xbox 360** or **Kinect for Windows** device

### Software Requirements
Windows 7 or Windows 8
Microsoft .NET Framework 4



## Getting Started
1.	Make sure the Kinect drivers are installed on your computer. You can download the drivers at http://www.microsoft.com/en-us/download/details.aspx?id=40277.
2.	Connect Kinect device to the computer. 
3.	Start KineCTRL.exe. You should see something like this:

![Main Screen of the application](/images/mainscreen.png)


## Main Menu

### Sensor Status
Here you can see the status of the Kinect sensor. Sensor should be active to properly use the application.
![Sensor Active](/images/sensoractive.png) **Sensor Active**

![Waiting For Sensor](/images/sensorwaiting.png) **Waiting For Sensor**

![Sensor Error](/images/sensorerror.png) **Sensor Error**


### Recognizers
Here you can turn recognizers on and off.

![Gesture Recognition on/off](/images/recog_gesture.png) **Gesture Recognition on/off.** This enables/disables recognition of all defined gestures as well as execution of the defined keyboard actions.

![Voice Recognition on/off](/images/recog_voice.png) **Voice Recognition on/off.** With Voice Recognition enabled you can control the application using voice commands.

![HandMouse Recognition on/off](/images/recog_handmouse.png) **HandMouse Recognition on/off.** This enables/disables the HandMouse function, which you can use to control the mouse cursor using hand movements.


### HandMouse Mode
Here you can set which hand is tracked to control the mouse cursor.

### Tilt Control
Here you can adjust the tilt of the Kinect sensor. 

### Profiles
Here you can manage the user profiles. A profile stores **all of your gestures and settings**. The profile is automatically saved when you close the application, or when another profile is loaded.
•	You can load a profile by **double clicking on its name**, or by selecting a profile and clicking the **Load** button.
•	You can copy selected profile by clicking the **Copy** button.
•	You can delete selected profile by clicking the **Delete** button.
•	You can create blank profile by clicking the **New** button.
•	You can rename selected profile by typing a new name into the text field and clicking the **Rename** button.


## Gesture Recognizer

When the gesture recognition is enabled in the main menu, you can see the result of each recognizer in the boxes in the lower part of the window. Once a dynamic gesture is recognized, its name appears in the Result box and starts to slowly fade away. If a posture is being recognized, its name is shown in the Result box. If more than one posture is being recognized, name of each posture is shown.

In the black screen you can see the real-time render of your skeleton.

### Tips for the Best Results

For the best recognition results, we recommend to place the sensor so there is **no window or a strong source of light** in the sensor’s field of view.

Make sure the **tilt of the sensor** is properly adjusted so the entire skeleton is being tracked. If some portion of the skeleton couldn’t be tracked, the inferred bones are rendered with a thin grey line. Tracked bones are rendered with a thick green line. Try to find the right tilt and placement within the room so that all the bones are rendered green.

If your skeleton is being rendered in red color, you are not an active user. **Raise both hands** to gain focus and become active.


### Gesture Manager

Here you can manage your gestures. Click on the type of the gesture you wish to manage. All defined gestures of the selected type will appear in the list. Click on a gesture to review its definition.

## Creating a New Body / Left Hand / Right Hand Gesture

-	Select the type of the gesture you wish to create.
-	Click the **Add New Gesture** button.
-	The Gesture Recorder will wait for you to walk into the sensor’s field of view. You will see the text **WAITING FOR USE** in the Gesture Recorder Status box.
-	When you walk into the sensor’s field of view, the Recorder will wait for you to find the right position. You will see the text **PLEASE STAND STILL**. Part of your skeleton will be highlighted according to the chosen type of the gesture.
-	Once you’re in position, just stand still and wait. The status will soon change to **PERFORM GESTURE NOW**.
-	Now you can demonstrate your intended gesture. While demonstrating, you will see the text **RECORDING GESTURE**.
-	When you’re done with the demonstration, you will see the text **GESTURE SAVED**. The gesture will appear in the list and you can immediately review the recording or edit your gesture.

## Creating a New Posture
-	Select **Postures** from the list of gesture types.
-	Click the **Add New Gesture** button.
-	The Gesture Recorder will wait for you to walk into the sensor’s field of view. You should see the text **WAITING FOR USER** in the Gesture Recorder Status box.
-	When you walk into the sensor’s field of view, the Recorder will wait for you to demonstrate your intended pose. You should see the text **PLEASE STAND STILL**. 
-	Stand in the intended pose and wait. The posture will soon be captured and you will see the text **POSTURE SAVED**. The posture will appear in the list and you can immediately review the captured pose or edit your posture.

## Editing Gestures
-	To change the name of a gesture, type a new name into the text field and click the **Rename** button.
-	To set a new definition for the gesture, click the **Redefine** button.
-	To delete gesture, click the **Delete** button.

## Setting Keyboard Actions
-	Keyboard action, which will be executed when you perform the gesture, is shown in the Gesture Info box, right next to the **Action** label.
-	To set a keyboard action, click on the bold text. This text will read either **Undefined** or the already assigned key will be shown. Once you click on the bold text, it will immediately change to **press a key…**
-	Press the intended key on your keyboard. You can also press a combination of keys using **Ctrl**, **Shift**, **Alt** or **Win** keys.
-	To remove a keyboard action, click on the action with **right mouse button**.

> #### Notice
> Don’t forget to redefine your **default posture**! It is your neutral pose that executes **no action**.
> To do so, click on the **Postures** in the list of gesture types. Click on the **Default** in the list of postures. Click the **Redefine** button in the Gesture Info box. Walk in front of the sensor and stand **straight**, with **hands along the body** and **legs together**. Be **relaxed**.




## Advanced Settings
Here you can adjust the threshold values for the recognition algorithms. The default values should yield the best recognition results; however, sometimes you may need to adjust them, based on the environment or your preferences.


### Body Recognizer Settings / Left & Right Hand Recognizer Settings

**Detection Sensitivity** is the threshold for the motion detection. Lower value means the detection will be more sensitive, so subtler movements will be classified as candidate gestures. Higher value means less sensitive detection, so movements will have to be more distinct to be classified as candidate gestures.

**Confidence Threshold** is the value that determines if a keyboard action will be sent to the operating system when a gesture is recognized. Lower value means that performed gestures will have to be more accurate to execute keyboard action. Higher value allows for less accurate gestures.


### Posture Recognizer Settings

**Definition tolerance** is the distance threshold for key joints selection from the template posture. Higher value means that the position of the joint in the template posture will have to differ more when compared to default posture to be considered as key joint. This results in fewer key joints being selected. Lower value results in the opposite.

**Confidence threshold** is the distance used when the candidate posture is being compared to the template posture. If all key joints in candidate posture lie within this distance in the template posture, the posture is recognized. Thus, lower value means the candidate posture will have to be more accurate. Higher value allows for less accurate postures.



### Gesture Recorder Settings

**Detection Sensitivity** is the threshold for motion detection while recording new gestures. Lower value means the recorder will be more sensitive and the recording will be started with a subtler motion. Higher value means less sensitive recorder, so the motion will have to be more distinct to start the recording.

**Recorder Ready Threshold** is the value that determines how steady the user has to stand in order to switch from the PLEASE STAND STILL state to the PERFORM GESTURE NOW state. Lower value means the user will have to stand steadier. Higher value allows for the opposite.



### Speech Recognizer Settings

**Confidence Threshold** is the value below which the speech is treated as if it hadn't been heard. With higher value means the speech will have to be more clear and accurate. Lower value allows for more indistinct speech.



### HandMouse Recognizer Settings
**Sensitivity** is the value for the mouse cursor control. Higher value means the cursor will be more sensitive to hand movements. Lower value means the opposite. 



### Debugging
To start the Debug Mode, click the **Show Debugging Window** button.

Check **Execute Key Presses** to send the keyboard events into the operating system when a gesture is recognized.

Check **Multiply Key Presses** to send each keyboard event multiple times. This is useful in DirectX games, for more information see section Known Issues.


## Debug Mode
The special Debugging window is used to analyze the behavior of the recognizer. Select a recognizer from the dropdown menu and you will be able to see the **Position change** in the last 15 frames, the recognizer **Status**, the last **Result** of the recognition, and the **Confidence** of the last recognized gesture. 

On the right side you can compare the last performed gesture with the result of the recognition, even if the result didn’t qualify for a keyboard event.

In order to view this information, the gesture recognition has to be activated.

![Debug Mode](/images/debugmode.png)