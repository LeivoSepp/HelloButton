using System;
using Windows.ApplicationModel.Background;
using Windows.Devices.Gpio;
using Windows.Foundation;

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

            pinGreen = gpio.OpenPin(LED_PIN_GEEN);
            pinRed = gpio.OpenPin(LED_PIN_RED);
            pinRed.SetDriveMode(GpioPinDriveMode.Output);
            pinGreen.SetDriveMode(GpioPinDriveMode.Output);

            btnGreen = gpio.OpenPin(BTN_GREEN);
            btnGreen.SetDriveMode(GpioPinDriveMode.InputPullUp);
            btnGreen.DebounceTimeout = TimeSpan.FromMilliseconds(50);
            btnRed = gpio.OpenPin(BTN_RED);
            btnRed.SetDriveMode(GpioPinDriveMode.InputPullUp);
            btnRed.DebounceTimeout = TimeSpan.FromMilliseconds(50);

            btnGreen.ValueChanged += Btn_ValueChanged;
            btnRed.ValueChanged += Btn_ValueChanged;
        }

        private void Btn_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            GpioPin led = null;
            if (sender.PinNumber == BTN_GREEN) led = pinGreen;
            if (sender.PinNumber == BTN_RED) led = pinRed;

            GpioPinValue pinValue = led.Read();
            // toggle the state of the LED every time the button is pressed
            if (e.Edge == GpioPinEdge.FallingEdge)
            {
                pinValue = (pinValue == GpioPinValue.Low) ?
                    GpioPinValue.High : GpioPinValue.Low;
                led.Write(pinValue);
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
