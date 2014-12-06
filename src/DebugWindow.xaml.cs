using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Kinect;
using KineCTRL.Streams;

namespace KineCTRL
{
    /// <summary>
    /// Interaction logic for DebugWindow.xaml
    /// </summary>
    public partial class DebugWindow : Window
    {
        /// <summary>
        /// Skeleton renderer for gesture comparer
        /// </summary>
        public SkeletonRenderer RendererDebugPerformed;

        /// <summary>
        /// Skeleton renderer for gesture comparer
        /// </summary>
        public SkeletonRenderer RendererDebugOriginal;

        /// <summary>
        /// Skeleton renderer for gesture comparer
        /// </summary>
        public string RecognizerInfoMode;

        public DebugWindow()
        {
            InitializeComponent();

            RendererDebugPerformed = new SkeletonRenderer(ImageOutputDebugPerformed);
            RendererDebugOriginal = new SkeletonRenderer(ImageOutputDebugOriginal);
        }

        private void Debug_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void CboxRecognizerInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Reset Recognizer Info Panel
            if (RendererDebugOriginal != null)
            {
                // Stop replaying recordings
                RendererDebugPerformed.RenderRecordingStop();
                RendererDebugOriginal.RenderRecordingStop();

                // Clear textboxes
                TbPositionChange.Text = "-";
                TbStatus.Text = "-";
                TbResult.Text = "-";
                TbConfidence.Text = "-";
            }

            if (CboxRecognizerInfo.SelectedIndex == 0)
            {
                RecognizerInfoMode = Const.OFF;
            }
            if (CboxRecognizerInfo.SelectedIndex == 1)
            {
                RecognizerInfoMode = Const.BODY;
            }
            if (CboxRecognizerInfo.SelectedIndex == 2)
            {
                RecognizerInfoMode = Const.LEFT_HAND;
            }
            if (CboxRecognizerInfo.SelectedIndex == 3)
            {
                RecognizerInfoMode = Const.RIGHT_HAND;
            }
        }

        private void Debug_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Reset combobox
            CboxRecognizerInfo.SelectedIndex = 0;

            // Cancel window closure
            e.Cancel = true;

            // Hide window
            Hide();
        }
        
    }
}
