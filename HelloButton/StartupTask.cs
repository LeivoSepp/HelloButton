using System;
using Windows.ApplicationModel.Background;
using Windows.Devices.Gpio;

namespace HelloButton
{
    public sealed class StartupTask : IBackgroundTask
    {
        int LED_PIN_GEEN = 47;
        int LED_PIN_RED = 35;
        int BTN_GREEN = 5;
        int BTN_RED = 6;
        GpioPin pinGreen;
        GpioPin pinRed;
        GpioPin btnGreen;
        GpioPin btnRed;
        private void init()
        {
            var gpio = GpioController.GetDefault();
            if (gpio == null)
            {
                return;
            }
            pinGreen = gpio.OpenPin(LED_PIN_GEEN);
            pinRed = gpio.OpenPin(LED_PIN_RED);
            pinRed.Write(GpioPinValue.High);
            pinRed.SetDriveMode(GpioPinDriveMode.Output);
            pinGreen.Write(GpioPinValue.High);
            pinGreen.SetDriveMode(GpioPinDriveMode.Output);

            btnGreen = gpio.OpenPin(BTN_GREEN);
            btnGreen.SetDriveMode(GpioPinDriveMode.InputPullUp);
            btnGreen.DebounceTimeout = TimeSpan.FromMilliseconds(50);
            btnRed = gpio.OpenPin(BTN_RED);
            btnRed.SetDriveMode(GpioPinDriveMode.InputPullUp);
            btnRed.DebounceTimeout = TimeSpan.FromMilliseconds(50);

            btnGreen.ValueChanged += BtnGreen_ValueChanged;
            btnRed.ValueChanged += BtnRed_ValueChanged;

        }

        private void BtnGreen_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            GpioPinValue pinValue = pinGreen.Read();
            // toggle the state of the LED every time the button is pressed
            if (e.Edge == GpioPinEdge.FallingEdge)
            {
                pinValue = (pinValue == GpioPinValue.Low) ?
                    GpioPinValue.High : GpioPinValue.Low;
                pinGreen.Write(pinValue);
            }
        }

        private void BtnRed_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            GpioPinValue pinValue = pinRed.Read();
            // toggle the state of the LED every time the button is pressed
            if (e.Edge == GpioPinEdge.FallingEdge)
            {
                pinValue = (pinValue == GpioPinValue.Low) ?
                    GpioPinValue.High : GpioPinValue.Low;
                pinRed.Write(pinValue);
            }
        }

        internal static BackgroundTaskDeferral Deferral = null;
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            Deferral = taskInstance.GetDeferral();
            init();
        }
    }
}
