# Lesson #3 Hello Button
This project uses Raspberry PI, Windows 10 IoT Core and two buttons to toggle the onboard leds.

* Lesson #1 in this series:https://github.com/LeivoSepp/Lesson1-HelloBlinky
* Lesson #2 in this series: https://github.com/LeivoSepp/Lesson2-HelloBlinkyRedGreen
* Please check my blog here: http://internetofthing.io/: 

# Why try this program?
* You will learn how to use buttons (using pins as an input for the buttons)
* You will learn how to turn the onboard LEDs OFF or ON with each button press
* You will learn hot to use Event handler, which tracks your buttons
* You will learn how to write one method to use in both buttons
* You will learn how to make a program that runs infinitely without the while(true) statement

# Let's look at the code from beginning
Since this project is the third Lesson in our Raspberry series, some of the code will be explained less thoroughly as it has already been covered earlier.
Let's start from the beginning.

In this project we need 4 pins: 2 for LEDs and 2 for buttons.

## Setting variables
As usual, we start by creating variables. This time there's 4 of them: two for LEDs and two for buttons.
We will also create four variables for our pins themselves.
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
Take a look at this picture, GPIO is all the pins and pin is one particular pin. Raspberry PI has two special pins which are to control the green and red LED integrated on the board. The green LED’s pin number is 35 and the red’s pin number is 47. 
![image](https://cloud.githubusercontent.com/assets/13704023/22621382/13b33e60-eb2b-11e6-9776-cf6ca9691280.png)

## Init method
This time the init method is a bit longer than earlier. It has four important things to do:

1. Creating a gpio controller to let our code know all about the pins
2. Setting some parameters for LED pins. For example we'll be setting the correct pin number to each pin and declaring them as an output.
3. Setting parameters for buttons:
	1. What is the pin number? "your pin number is 5" **btnGreen = gpio.OpenPin(BTN_GREEN);**
	2. Is that pin an input or an output? "this pin is an input" **btnGreen.SetDriveMode(GpioPinDriveMode.InputPullUp);**
		* The InputPullUp means that inside the Raspberry, the pin will be connected to +3V through the resistor
		* To use this pin as a button, you need to connect it to the ground (GND)
	3. What if the button sends many signals with a single press? "this pin accepts a new value change only after 50ms has passed" **btnGreen.DebounceTimeout = TimeSpan.FromMilliseconds(50);**
		* this timeout is needed because all buttons usually generate 2 or 3 signals in one press
4. Registering event handlers for the buttons. **btnGreen.ValueChanged += Btn_ValueChanged;**
	* Event handler is a small program which tracks the button state. If someone pushes the button, then the pin's state will change and the code inside Btn_ValueChanged will be executed.

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
This code will be executed only if either of the buttons are pressed. 

It has two special input arguments: **sender** and **e**:

1. **sender** always represents the object that started the event. For example, if btnRed has been pressed, then the sender is *btnRed*.
2. **e** represents the event itself. In our example **e** means that the pin value changed from High to Low, which means *FallingEdge*.

Now let's see the code inside the event handler.

1. We will create a new pin type variable named *led*. **GpioPin led = null;**
2. We will check the sender, if the sender's pin number is equal to BTN_GREEN then we assign the green LED's pin to variable led. **if (sender.PinNumber == BTN_GREEN) led = pinGreen;**
3. If the sender's pin number is equal to BTN_RED then we assign the red LED's pin to variable led. 
4. We will create a new pin value type variable pinValue, read the LED's pin value and assign the value to it. **GpioPinValue pinValue = led.Read();**
5. Then comes an IF statement, which checks if the button is pressed. **e.Edge == GpioPinEdge.FallingEdge**
6. Finally, our friend the ternary condition ( ? : ) that does two things:
	1. Checks whether or not the LED is turned OFF **pinValue == GpioPinValue.Low** if yes, then it assigns High to pinValue, otherwise it assigns Low to pinValue
	2. And after that, changes the LED's state according to the pinValue, which was just set in the previous line **led.Write(pinValue);**

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

This time, the method Run has some special commands, which prevents it from stopping.

These two lines keep method Run in the running state forever

1. Before the method Run, there's just one row: **internal static BackgroundTaskDeferral Deferral = null;**
2. Inside the method Run is again, just one row: **Deferral = taskInstance.GetDeferral();**

The code executes **init();** and then just stays as it is and doing nothing, but only checking the buttons statuses. 

```C#
        internal static BackgroundTaskDeferral Deferral = null;
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            Deferral = taskInstance.GetDeferral();
            init();
        }
```
