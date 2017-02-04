# Lesson3-HelloButton
This project is using Raspberry PI, Windows 10 IoT Core and two buttons to toggle onboard leds.

# Why is it worth to try this program?
* You will learn, how to use buttons (this mean using pins as an input)
* You will learn, how to turn onboard LEDs ON and OFF in each button press
* You will learn, hot to use Event handler, which track your buttons
* You will learn how to write one method to use in both buttons
* You will learn, hot to make forever lasting program without while(true) statement

# Lets look the code from the beginning
As this project is third Lesson in our Raspberry series, we will some code more lightly as they are alredy explained earlier.
But still, lets start from the beginning.

In this project we need 4 pins, 2 for LEDs and 2 for buttons.

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

This time the init method is a bit longer than earlier. It has four important things to do.

1. Creating gpio controller to let our code know all about the pins
2. Setting some parameters for LED pins. For example we are setting according pin-number to each pin and setting them as an aoutput.
3. Setting parameters for buttons:
	1. what is the pin number? "your pin number is 5" btnGreen = gpio.OpenPin(BTN_GREEN);
	2. is that pin input or output? "this pin is input" btnGreen.SetDriveMode(GpioPinDriveMode.InputPullUp);
		* the InputPullUp mean, that inside the Raspberry the pin will be connected to +3V through the resistor
		* to use this pin as a button, you need to connect to ground (GND)
	3. what if the button sends many signals in one press? "this pin accepts new value change after 50ms"
		* this timeout needed, because all buttons are generating usually 2 or 3 signals in one button press
4. Registering event handlers for the buttons.
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
