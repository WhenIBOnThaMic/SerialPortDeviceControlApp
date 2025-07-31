# Serial Port Device Control Application

This application allows users to communicate with a device via a serial port, visualize data through graphs, and control the device's operations. 

## Features

- Detect and select available COM ports.
- Connect and terminate connections with the device.
- Receive data from the device and display it in real-time graphs.
- Send commands to the device and handle responses.

## Project Structure

```
SerialPortDeviceControlApp
├── src
│   ├── MainForm.cs          # Main user interface for the application
│   ├── SerialPortManager.cs  # Manages serial port communication
│   ├── DeviceController.cs    # Controls the device and handles commands
│   ├── GraphRenderer.cs       # Renders graphs based on received data
│   └── Program.cs             # Entry point of the application
├── README.md                  # Documentation for the project
└── SerialPortDeviceControlApp.csproj  # Project configuration file
```

## Setup Instructions

1. Clone the repository or download the source code.
2. Open the project in your preferred C# development environment.
3. Restore any necessary dependencies.
4. Build the project to ensure all components are correctly configured.

## Usage

1. Launch the application.
2. Select the desired COM port from the dropdown menu.
3. Click the "Connect" button to establish a connection with the device.
4. Use the interface to send commands and visualize the data received from the device.
5. Click "Disconnect" to terminate the connection when finished.

## Dependencies

- .NET Framework (version required)
- Any additional libraries used for serial communication or graph rendering.

## License

This project is licensed under the MIT License - see the LICENSE file for details.