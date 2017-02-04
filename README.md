# Lesson #3 Hello Button
This project is using Raspberry PI, Windows 10 IoT Core and two buttons to toggle onboard leds.

* Lesson #1 in this series are:https://github.com/LeivoSepp/Lesson1-HelloBlinky
* Lesson #2 in this series are: https://github.com/LeivoSepp/Lesson2-HelloBlinkyRedGreen
* Please check by blog here: http://internetofthing.io/: 

# Why is it worth to try this program?
* You will learn, how to use buttons (this mean using pins as an input)
* You will learn, how to turn onboard LEDs ON and OFF in each button press
* You will learn, hot to use Event handler, which track your buttons
* You will learn, how to write one method to use in both buttons
* You will learn, how to make forever lasting program without while(true) statement

# Lets look at the code from beginning
As this project is third Lesson in our Raspberry series, we will some code more lightly as they are alredy explained earlier.
But still, lets start from the beginning.

In this project we need 4 pins, 2 for LEDs and 2 for buttons.

## Setting variables
As usual, we are starting to create variables, this time 4 of them. 2 for LEDs and two for buttons.
Also, we are creating variables for all four pins.
```C#
        int LED_PIN_GEEN = 47;
        int LED_PIN_RED = 35;
        int BTN_GREEN = 5;
        int BTN_RED = 6;
        GpioPin pinGreen;
        GpioPin pinRed;
        GpioPin btnGreen;
        GpioPin btnRed;
```

## Init method
This time the init method is a bit longer than earlier. It has four important things to do.

1. Creating gpio controller to let our code know all about the pins
2. Setting some parameters for LED pins. For example we are setting according pin-number to each pin and setting them as an aoutput.
3. Setting parameters for buttons:
	1. what is the pin number? "your pin number is 5" **btnGreen = gpio.OpenPin(BTN_GREEN);**
	2. is that pin input or output? "this pin is input" **btnGreen.SetDriveMode(GpioPinDriveMode.InputPullUp);**
		* the InputPullUp mean, that inside the Raspberry the pin will be connected to +3V through the resistor
		* to use this pin as a button, you need to connect to ground (GND)
	3. what if the button sends many signals in one press? "this pin accepts new value change after 50ms" **btnGreen.DebounceTimeout = TimeSpan.FromMilliseconds(50);**
		* this timeout needed because all buttons are generating usually 2 or 3 signals within one press
4. Registering event handlers for the buttons. **btnGreen.ValueChanged += Btn_ValueChanged;**
	* Event handler is a small program which track the button state. If someone push the button, then the pin's state will change and the code inside Btn_ValueChanged will be executed.

```C#
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
```

## Event handler code
This code will be executed only, if either of the button are pressed. 

It has some special input arguments **sender** and **e**:

1. **sender** is represent always the object, which raises the event. For example, if btnRed has been pressed, then the sender is *btnRed*.
2. **e** is representing the event itself. In our example the **e** means that pin value changed from High to Low which means *FallingEdge*.

Now lets see the code inside the event handler.

1. We will create a new pin-type variable named *led*. **GpioPin led = null;**
2. We will check the sender, if sender's pin number equal to BTN_GREEN then we assign green LED's pin to variable led. **if (sender.PinNumber == BTN_GREEN) led = pinGreen;**
3. If sender's pin number equal to BTN_RED then we assign red LED's pin to variable led. 
4. We will create new pin-value type variable pinValue and reading LED pin value and assigning to it. **GpioPinValue pinValue = led.Read();**
5. Now there is one IF statement, which just checking is the button pressed. **e.Edge == GpioPinEdge.FallingEdge**
6. Now there is our friend ternary condition ( ? : ) checking:
	1. If LED is turned OFF **pinValue == GpioPinValue.Low** then assign High to pinValue else assign Low to pinValue
	2. change the LED's state according to piValue, which has been set just in previous line **led.Write(pinValue);**

```C#
        private void Btn_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            GpioPin led = null;
            if (sender.PinNumber == BTN_GREEN) led = pinGreen;
            if (sender.PinNumber == BTN_RED) led = pinRed;

            GpioPinValue pinValue = led.Read();
            // toggle the state of the LED every time the button is pressed
            if (e.Edge == GpioPinEdge.FallingEdge)
            {
                pinValue = (pinValue == GpioPinValue.Low) ?  GpioPinValue.High : GpioPinValue.Low;
                led.Write(pinValue);
            }
        }
```

## void Run

This time the method Run has some special commands, which prevents it from stopping.

These two lines keep method Run in running state forever

1. before Run just one row: **internal static BackgroundTaskDeferral Deferral = null;**
2. inside Run one row: **Deferral = taskInstance.GetDeferral();**

The code executes **init();** and then just stays as it is and doing nothing, but only checking the buttons statuses. 

```C#
        internal static BackgroundTaskDeferral Deferral = null;
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            Deferral = taskInstance.GetDeferral();
            init();
        }
```
