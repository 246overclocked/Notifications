#Team 246 Overclocked Notifications System:

<h2>The Project:</h2>
The goal of this project is to create a simple solution for teams to send audiovisual feedback to the drive team about the state of the robot.  This is accomplished through a client GUI running on the driver station that receives UDP packets from the RoboRIO.  This system works without special configuration connected directly to the robot’s router and through the FMS.  Notifications are sent by a function call in the RoboRIO code, so teams can trigger notifications with whatever is important to their robot.  Teams can customize the severity of messages which changes the color of the messages, and, if desired, the soundfile that accompanies a notification.
![Empty GUI](http://imgur.com/3eNkCCr.png)
<h2>Installation:</h2>
<h3>Client:</h3>
If teams are happy with the current configuration, the included executable (found in the zip file Driver Station Alert System\Driver Station Alert System\FRC Driver Alert\Driver Alert Station) will work out of the box.  The default port is 5801. If teams want to change aspects of the GUI, they will need to rebuild the project using visual studio.  In particular, the listening port can be changed on line 17 of AlertsViewModel.cs (Driver Alert Station/ViewModels/AlertsViewModel.cs)  Additionally the name of the window can be changed in line 14 of MainWindow.xaml (Driver Alert Station/Views/MainWindow.xaml)

<h3>Robot:</h3>
First, AlertMessage.java and UdpService.java (Java/UdpAlertService/src/roborio/) should be added to the robot code.  Next, the connection needs to be initialized.  Detailed instructions for initializing the connection can be found at the top of UdpService.java found in Java\UdpAlertService\src\Roborio\UdpService.java.  Once the UdpService is initialized, messages can be sent very simply with a simple line.  For example: UdpAlertService.sendAlert(new AlertMessage("Entering Robot-Centric Mode").playSound("r2d2.wav"));
This line sends an alert saying “Entering Robot-Centric Mode” and plays a soundfile named r2d2.wav in the Driver Alert Station\Sounds folder(where all sound files should be put).  Severity, subsystem, and other properties of a notification can also be customized.  The full details of these options are also detailed at the top of the UdpService file.
