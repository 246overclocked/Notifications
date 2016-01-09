package udpalertservice;

import java.io.IOException;
import java.net.InetAddress;
import java.net.DatagramSocket;
import java.net.DatagramPacket;
import java.util.Arrays;
import java.util.List;

/**
 *
 * @author Dave Anderson
 *
 */
public class UdpService {

    // In the robot initialization code, call 'initialize' with the host and port where the alert service can be found.
    // This should only be called ONCE, when the robot starts up, and the call should look something like this: 
    // UdpService.initialize("driverstation.local", 5801);
    // or
    // UdpService.initialize("10.2.46.50", 5801);
    //
    // After the service is initialized, the robot code can send alert messages to the alert service whenever it wants to.
    // Sending a minimal message looks something like this:
    // UdpService.sendAlert(new AlertMessage("Minimal alert message"));
    // 
    // You can cause an alert message to play a sound when it is received, by sending the name of a wav file to play along with
    // the alert message, thus:
    // UdpService.sendAlert(new AlertMessage("This plays a sound").playSound("AlertSound.wav"));
    //
    // Please note that if the sound file is not present on the alert service laptop, the message will be displayed but no sound will be played
    //
    // Alert messages have other optional fields as well.  These fields are Severity, Category, Subsystem and Details
    // UdpService.sendAlert(new AlertMessage("This message has optional fields").playSound("AlertSound.wav").severity(AlertMessage.Severity.DEBUG).category("myCategory").subsystem("mySubsystem").addDetail("This is some message detail"));
    //
    // Severity levels are defined in AlertMessage.java, and are intended to be used to filter messages by importance (such as debug messages sent during development and testing of the robot code)
    // Category, Subsystem and Detail fields are passed along to the alert service, but are not currently used by the alert service code

    public static void initialize(String alertServiceHost, int alertServicePort)
    {
    	UdpService._alertServiceHost = alertServiceHost; 
    	UdpService._alertServicePort = alertServicePort;

        try
        {
            _alertServerAddress = InetAddress.getByName(_alertServiceHost);
            _alertSocket = new DatagramSocket();
            _alertSocket.setBroadcast(false);
            _alertSocket.setReuseAddress(true);
        }
        catch (Exception e)
        {
            _alertSocket = null;
        }
    }
    
    private static InetAddress _alertServerAddress;
    private static int _alertServicePort;
    private static DatagramSocket _alertSocket = null;
    private static String _alertServiceHost;
    
    public static void sendAlert(AlertMessage alertMessage)
    {
    	if (_alertSocket == null) initializeAlertService();
    	if (_alertSocket == null) return;
        String xmlString = alertMessage.toXml();
        DatagramPacket packet = new DatagramPacket(xmlString.getBytes(), xmlString.length(), _alertServerAddress, _alertServicePort);
        try {
            _alertSocket.send(packet);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
    
    public static void close()
    {
        try
        {
            if (_alertSocket != null) _alertSocket.close();
        }
        catch (Exception e) {}
    }
}
