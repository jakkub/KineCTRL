   M S B A M L   `   `   `  ����   KineCTRL    KineCTRL.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -   $
��Main��  � Title$  KineCTRL��$	��836��$
��1092�� � WindowStartupLocation$ CenterScreen=� � 	Resources  �� 
TargetType   
ImageHover �� Triggers P� Property P� Value �� Property �� Value��%Z   &	        �� " �� P�  �@J�IsMouseOver . . ���  �@J�Opacity$	 0.8Q�.��� 
������ .+�� -   	 �Name$	 GridMain��
 � RowDefinitions
 �� �� $
��Auto �.+��� -   $��	219,3,0,5q�$	��843��$	��780��.@��� -   $��TabItemGestureRecognizer��$��Gesture Recognizer��.��� 
������  � ColumnDefinitions �� $	��304 ��� $	��704 �.+��� -   $��ImageOutputMain��$	��600��$
��Left=�$
��Fill=�$	��Top=�$	��800��$��16,6,0,0q�$��2���� -   $��comboBoxOutputStream��$��23��$
��Left=�$��	399,6,0,0q�$	��Top=�$	��113�� �� SelectedIndex$ 0��$��1��$y�Hidden=�.@��� $��Skeleton Stream���� $��Color Stream���� $��Depth Stream���� $��Body Recognizer Result��$
��Left=�$��282,612,0,0q�$	��Top=�$��61��$	��268��$��2��.���� -    ��Name$ LblBodyRecognizerResult��$�� ��$��34(�$��Normal&����	#FF1F1F1F$��48��$
��Left=�$��
0,-6,-2,-3q�$	��Top=�$	��258��$��Center=�$	��5,0q�$
��Bold"� J� OpacityMask
 �������Z� �� $!��Left Hand Recognizer Result��$
��Left=�$��
10,612,0,0q�$	��Top=�$��61��$	��267��.���� -   $ LblLHRecognizerResult��$�� ��$��34(�$��Normal&����	#FF1F1F1F$��48��$
��Left=�$��
0,-6,-2,-3q�$	��Top=�$	��257��$��Center=�$	��5,0q�$
��Bold"���Z� �� $"��Right Hand Recognizer Result��$
��Left=�$��251,612,0,0q�$	��Top=�$��61��$	��268��$��1��.���� -	   $ LblRHRecognizerResult��$�� ��$��34(�$��Normal&����	#FF1F1F1F$��48��$
��Left=�$��	0,-6,0,-3q�$	��Top=�$	��247��$��Center=�$	��5,0q�$
��Bold"���Z� �� $��Posture Recognizer Result��$
��Left=�$��
10,683,0,0q�$	��Top=�$��61��$	��813��$��2��.���� -
   $  LblPostureRecognizerResult��$�� ��$��34(�$��Normal&����	#FF1F1F1F$��48��$
��Left=�$��
0,-6,-2,-3q�$	��Top=�$	��803��$��Center=�$	��5,0q�$
��Bold"���Z� �� -    ��Name$ TbTipBecomeActive��$��1��$
��Left=�$��193,574,0,0q�$
��Wrap=�$,��&Tip: Raise both hands to become active��$	��Top=�$��18(����	#FF54C104$y�Hidden=��� -   $��TabItemGestureManager��$��Gesture Manager�� �� 
IsSelected .  .��� 
������  �� $
��275* ��� $
��747* �.+��� -   $��LboxGestures��$��12,105,52,161q��� -   $��ImageOutputManager��$	��480��$
��Left=�$��183,105,0,0q�$
��Fill=�$	��Top=�$	��640��$��2���� -   $��BtnNewGesture��$��Add New Gesture��$��35��$��
0,601,52,0q�$	��Top=�$	��160��$��Right=�$��14(�	� -   $��RbBodyGestures��$��Body Gestures��$��16��$
��Left=�$��	12,11,0,0q�$	��Top=�	� -   $��RbLHGestures��$��Left Hand Gestures��$��16��$
��Left=�$��	12,33,0,0q�$	��Top=�	� -   $��RbRHGestures��$��Right Hand Gestures��$��16��$
��Left=�$��	12,55,0,0q�$	��Top=�	� -    	�Name$ 
RbPostures��$��Postures��$��16��$
��Left=�$��	12,76,0,0q�$	��Top=��� $��Gesture Recorder Status��$
��Left=�$��
183,16,0,0q�$	��Top=�$��66��$	��640��$��2��.��� .+��� -   $ LblRecorderStatus��$��STAND BY��$��36(�$��Normal&����	#FF383085$��48��$
��Left=�$��	10,-4,0,0q�$	��Top=�$	��608��$��Center=�$��Segoe UI Semibold)�$	��5,0q���Z� �� -    ��Name$ GboxGestureInfo��$��Gesture Info��$
��Left=�$��183,601,0,0q�$	��Top=�$	��138��$	��324��$��2��.��� .+��� -   $ TbSetAction�� � Cursor$
 Hand��|�. $
��Bold"�$��23��$
��Left=�$��	55,50,0,0q�$��	Undefined��$	��Top=�$��14(��� -   $ LblGestureAction��$��Action:��$��28��$
��Left=�$��2,45,0,0q�$	��Top=�$��14(��� -   $ LblGestureName��$��Name:��$��28��$
��Left=�$��4,9,0,0q�$	��Top=�$��14(��� -    ��Name$ TboxGestureName��$��28��$
��Left=�$��	55,10,0,0q�$	��Top=�$	��160��$��14(��� -    ��Name$ BtnSaveGestureName��$��Rename��$��28��$
��Left=�$��220,10,-1,0q�$	��Top=�$��86��$��14(��� -   $ BtnDeleteGesture��$��Delete��$��28��$
��Left=�$��
153,81,0,0q�$	��Top=�$��86��$��14(� � DataContext �� �� -   $ BtnRedefineGesture��$��Redefine��$
��Left=�$��	62,81,0,0q�$	��Top=�$��86��$��14(� J� RenderTransformOrigin$ 3.779,-1.16&�$��28���� -   $ TbDefaultPostureInfo��$��1��$
��Left=�$��
31,650,0,0q�$
��Wrap=�$$��This is your default posture. ��$	��Top=�$��14(�$��24��$	��178���� -    ��Name$ TabItemAdvancedSettings��$��Advanced Settings��.��� .+��� -   $ BtnResetSettingsToDefault��$��Reset to Default��$��681,705,10,10q�$��14(�$	��142��$��33���� $��Body Recognizer Settings��$
��Left=�$��	36,31,0,0q�$	��Top=�$	��179��$	��364��.��� .+��� -     ��Name$! SldrBodyRecognizerDetection��$
��Left=�$��	10,35,0,0q�$	��Top=�$	��336�� � Value$ 3Q� � Maximum$ 4Q� � Minimum$	 0.5Q� �� TickPlacement$ BottomRight=� �� TickFrequency$	 0.1Q� � LargeChange$	 0.3Q�  �� IsSnapToTickEnabled  . �� $
��Left=�$��	10,11,0,0q�$
��Wrap=�$��Detection Sensitivity��$	��Top=�$��14(��� -!   $" SldrBodyRecognizerConfidence��$
��Left=�$��10,111,-8,0q�$	��Top=�$	��336��$	 0.1Q�$ 2Q�$	 0.5Q�$	 0.1Q�$ BottomRight=�$	 0.3Q�  . �� $
��Left=�$��	10,87,0,0q�$
��Wrap=�$��Confidence Threshold��$	��Top=�$��14(��� -"   $$ TbBodyRecognizerDetectionValue��$
��Left=�$��
169,57,0,0q�$
��Wrap=�$	��Top=����� ! �� ElementName$!! SldrBodyRecognizerDetection��" �� Path$" Value��� -#   $% TbBodyRecognizerConfidenceValue��$
��Left=�$��169,133,0,0q�$
��Wrap=�$	��Top=����� $"! SldrBodyRecognizerConfidence��$" Value��� $+��%Left & Right Hand Recognizer Settings��$
��Left=�$��
438,31,0,0q�$	��Top=�$	��179��$	��364��.��� .+��� -$   $! SldrHandRecognizerDetection��$
��Left=�$��	10,35,0,0q�$	��Top=�$	��336��$	 1.3Q�$ 4Q�$	 0.5Q�$ BottomRight=�$	 0.1Q�$	 0.3Q�  . �� $
��Left=�$��	10,11,0,0q�$
��Wrap=�$��Detection Sensitivity��$	��Top=�$��14(��� -%   $" SldrHandRecognizerConfidence��$
��Left=�$��10,111,-8,0q�$	��Top=�$	��336��$	 0.1Q�$ 2Q�$ 1Q�$	 0.1Q�$ BottomRight=�$	 0.3Q�  . �� $
��Left=�$��	10,87,0,0q�$
��Wrap=�$��Confidence Threshold��$	��Top=�$��14(��� -&   $$ TbHandRecognizerDetectionValue��$
��Left=�$��
169,57,0,0q�$
��Wrap=�$	��Top=����� $!! SldrHandRecognizerDetection��$" Value��� -'   $% TbHandRecognizerConfidenceValue��$
��Left=�$��169,133,0,0q�$
��Wrap=�$	��Top=����� $"! SldrHandRecognizerConfidence��$" Value��� $!��Posture Recognizer Settings��$
��Left=�$��
36,244,0,0q�$	��Top=�$	��179��$	��364��.��� .+��� -(   $% SldrPostureRecognizerDefinition��$
��Left=�$��	10,35,0,0q�$	��Top=�$	��336��$	 0.2Q�$	 0.5Q�$
 0.05Q�$ BottomRight=�$
 0.01Q�$	 0.1Q�  . # � SmallChange$
# 0.01Q��� $
��Left=�$��	10,11,0,0q�$
��Wrap=�$��Definition Tolerance��$	��Top=�$��14(��� -)   $% SldrPostureRecognizerConfidence��$
��Left=�$��10,111,-8,0q�$	��Top=�$	��336��$
 0.05Q�$	 0.5Q�$	 0.2Q�$
 0.01Q�$ BottomRight=�$	 0.1Q�  . $
# 0.01Q��� $
��Left=�$��	10,87,0,0q�$
��Wrap=�$��Confidence Threshold��$	��Top=�$��14(��� $
��Left=�$��
169,57,0,0q�$
��Wrap=�$	��Top=����� *Value+$%! SldrPostureRecognizerDefinition���� $
��Left=�$��169,133,0,0q�$
��Wrap=�$	��Top=����� *Value+$%! SldrPostureRecognizerConfidence���� $#��HandMouse Recognizer Settings��$
��Left=�$��438,449,0,0q�$	��Top=�$	��107��$	��364��.��� .+��� -*   $ SldrHandMouseSensitivity��$
��Left=�$��	10,35,0,0q�$	��Top=�$	��336��$ 1Q�$ 3Q�$	 0.5Q�$ BottomRight=�$	 0.1Q�$	 0.3Q�  . �� $
��Left=�$��	10,11,0,0q�$
��Wrap=�$��Sensitivity��$	��Top=�$��14(��� $
��Left=�$��
169,57,0,0q�$
��Wrap=�$	��Top=����� *Value+$! SldrHandMouseSensitivity���� $ ��Speech Recognizer Settings��$
��Left=�$��
36,449,0,0q�$	��Top=�$	��107��$	��364��.��� .+��� -+   $$ SldrSpeechRecognizerConfidence��$
��Left=�$��	10,35,0,0q�$	��Top=�$	��336��$	 0.3Q�$ 1Q�$	 0.1Q�$ BottomRight=�$	 0.1Q�$	 0.3Q�  . �� $
��Left=�$��	10,11,0,0q�$
��Wrap=�$��Confidence Threshold��$	��Top=�$��14(��� $
��Left=�$��
169,57,0,0q�$
��Wrap=�$	��Top=����� *Value+$$! SldrSpeechRecognizerConfidence���� $��Gesture Recorder Settings��$
��Left=�$��438,244,0,0q�$	��Top=�$	��179��$	��364��.��� .+��� -,   $ SldrRecorderDetection��$
��Left=�$��	10,35,0,0q�$	��Top=�$	��336��$	 1.3Q�$ 4Q�$	 0.5Q�$ BottomRight=�$	 0.1Q�$	 0.3Q�  . �� $
��Left=�$��	10,11,0,0q�$
��Wrap=�$��Detection Sensitivity��$	��Top=�$��14(��� --   $ SldrRecorderReady��$
��Left=�$��10,111,-8,0q�$	��Top=�$	��336��$ 3Q�$ 8Q�$ 5Q�$	 0.2Q�$ BottomRight=�$	 0.5Q�  . $	# 0.2Q��� $
��Left=�$��	10,87,0,0q�$
��Wrap=�$��Recorder Ready Threshold��$	��Top=�$��14(��� $
��Left=�$��
169,57,0,0q�$
��Wrap=�$	��Top=����� *Value+$! SldrRecorderDetection���� $
��Left=�$��169,132,0,0q�$
��Wrap=�$	��Top=����� *Value+$! SldrRecorderReady���� $��	Debugging��$
��Left=�$��
36,589,0,0q�$	��Top=�$	��133��$	��364��.��� .+��� -.   $ BtnShowDebugWindow��$��Show Debugging Window��$��79,12,75,65q�$��14(��� -/   $ ��Name$$ CbDirectXmode��$��Multiply Key Presses��$
��Left=�$��	79,81,0,0q�$	��Top=��� -0   $$ CbExecuteKeyPresses��$��Execute Key Presses��$
��Left=�$��	79,60,0,0q�$	��Top=��� $��Profiles��$
��Left=�$��
10,453,0,0q�$	��Top=�$	��336��$	��206��.��� .+��� -1   % ��Name$% LboxProfiles��$��	5,76,2,33q��� -2   $ BtnLoadProfile��$
��Load��$
��Left=�$��5,49,0,0q�$	��Top=�$%��43                             ���� -3   $ BtnNewProfile��$	��New��$
��Left=�$��	53,49,0,0q�$	��Top=�$%��43                             ���� -4   $ BtnCopyProfile��$
��Copy��$
��Left=�$��
101,49,0,0q�$	��Top=�$��43���� -5   $ BtnDeleteProfile��$��Delete��$
��Left=�$��
149,49,0,0q�$	��Top=�$��43��$ 	0.349,3.5&��� -6   $ TboxRenameProfile��$
��Left=�$��23��$��	5,286,0,0q�& �� TextWrapping$
& Wrap=�$	��Top=�$	��129���� -7   $ BtnRenameProfile��$��Rename��$
��Left=�$��139,286,0,0q�$	��Top=�$��53��$ 0.528,2.045&��� -8   $ TbCurrentProfile��$
��Left=�$��	10,10,0,0q�$
��Wrap=�$	��Top=����	#F7383085$	��174��' �� TextAlignment$' Center=�$��25��$��18(��� -9   ( ��Name$( ImgLogo��$
��Left=�$��62��$��	18,11,0,0q�$	��Top=�$	��190��$��Images/simple_cyan.png���� -:   $ LblSkeletonStreamFPS��$��Label��$��Right=�$��0,336,1211,0q�$��25��$	��Top=��� -;   $ LblDepthStreamFPS��$��Label��$��28��$��Right=�$��0,377,1211,0q�$	��Top=��� -<   $ LblColorStreamFPS��$��Label��$
��Left=�$��19,357,0,448q��� $��Tilt Control��$
��Left=�$��
10,343,0,0q�$	��Top=�$	��103��$	��206��.��� $��Right=�$��82��$	��Top=�$	��186��$��	0,0,-2,-1q�.+��� -=   $ LblTilt��$
��Tilt��$��46��$
��Left=�$��	88,22,0,0q�$	��Top=�$ -2.692,-3.357&����	#FF54C104$��22(�$��52���� ->   $( 	ImgTiltUp��$
��Left=�$��32��$��52,7,0,0q�$	��Top=�$��32��$��Images/tilt_up.png��) � ToolTip$) Tilt Sensor Up��#��[  $
 Hand���� -?   $( ImgTiltDown��$
��Left=�$��32��$��	52,44,0,0q�$	��Top=�$��32��$��Images/tilt_down.png��$) Tilt Sensor Down��#��[  $
 Hand���� $��HandMouse Mode��$
��Left=�$��
10,263,0,0q�$	��Top=�$��71��$	��206��.��� $
��Left=�$��49��$	��Top=�$	��196��$��0,0,-2,0q�.+��� -@   $( ImgHandMouseModeLeft��$
��Left=�$��32��$��	62,10,0,0q�$	��Top=�$��32��$#��Images/handmouse_left_off.png��$) Left Handed Mouse Control��#��[  $
 Hand���� -A   $( ImgHandMouseModeRight��$
��Left=�$��32��$��	99,10,0,0q�$	��Top=�$��32��$$��Images/handmouse_right_off.png��$ ) Right Handed Mouse Control��#��[  $
 Hand���� $��Recognizers��$
��Left=�$��
10,169,0,0q�$	��Top=�$��83��$	��206��.��� $
��Left=�$��61��$	��Top=�$	��194��.+��� -B   $( ImgRecognitionStatus��$
��Left=�$��48��$��20,7,0,0q�$	��Top=�$��48��$ ��Images/recognition_off.png��$)  ��$
 Hand��#��[  �� -C   $( ImgSpeechStatus��$
��Left=�$��48��$��73,7,0,0q�$	��Top=�$��48��$��Images/speech_off.png��$
 Hand��$)  ��#��[  �� -D   $( ImgHandMouseStatus��$
��Left=�$��48��$��	126,7,0,0q�$	��Top=�$��48��$��Images/handmouse_off.png��$
 Hand��$)  ��#��[  �� $��Sensor Status��$
��Left=�$��	10,78,0,0q�$	��Top=�$	��206��$��83��.��� $
��Left=�$��61��$	��Top=�$	��194��.+��� -E   $( ImgSensorStatus��$
��Left=�$��48��$��
20,8,0,-10q�$	��Top=�$��48��$��Images/sensor_off.png��$)  ���� -F   $ LblTilt_Copy��$��Active��$��41��$
��Left=�$��95,8,0,0q�$	��Top=�$ -2.692,-3.357&����	#FF54C104$��22(�$��72��� -G   $	 GridSensorError�����	#B2FFFFFF$y�Hidden=�.+��� $
��Left=�$��228,368,0,0q�$
��Wrap=�$"��Please Connect Kinect Sensor��$	��Top=�$��74��$	��638��$��48(����	#FFC10C04