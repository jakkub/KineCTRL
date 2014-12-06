kinectrl
========

A customizable 3D gestural interface for Windows.


# Introduction
**KineCTRL** is a Windows application that serves as a gestural interface for the desktop environment. Using KineCTRL on your computer, you can **define your own set of full-body 3D gestures**, and use these gestures to **control the operating system** or any other Windows application.

KineCTRL lets you create four types of gestures: **Body Gestures**, **Left Hand Gestures**, **Right Hand Gestures** and **Postures**. Each defined gesture can be assigned a key, or a keyboard shortcut, which will be executed in the desktop environment once the gesture is recognized by the application. Body, Left Hand and Right Hand gestures are **dynamic gestures** based on the motion of a certain part of the body and their purpose is to emulate a key press. Postures are **static gestures** based on your current body pose and they are used to emulate a key being held.

With KineCTRL, you can also control the mouse cursor using **hand movements**.

## Minimum Hardware Requirements
**CPU:** Dual-core 2.66 GHz
**RAM:** 4 GB
**Kinect for Xbox 360** or **Kinect for Windows** device

## Software Requirements
Windows 7 or Windows 8
Microsoft .NET Framework 4



# Getting Started
1.	Make sure the Kinect drivers are installed on your computer. You can download the drivers at http://www.microsoft.com/en-us/download/details.aspx?id=40277.
2.	Connect Kinect device to the computer. 
3.	Start KineCTRL.exe. You should see something like this:

![Main Screen of the application](/images/mainscreen.png)


# Main Menu

## Sensor Status
Here you can see the status of the Kinect sensor. Sensor should be active to properly use the application.
![Sensor Active](/images/sensoractive.png) **Sensor Active**
![Waiting For Sensor](/images/sensorwaiting.png) **Waiting For Sensor**
![Sensor Error](/images/sensorerror.png) **Sensor Error**