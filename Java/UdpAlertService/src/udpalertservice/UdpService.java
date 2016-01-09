/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package udpalertservice;

import java.io.IOException;
import java.net.InetAddress;
import java.net.DatagramSocket;
import java.net.DatagramPacket;
import java.util.Arrays;
import java.util.List;

/**
 *
 * @author Dave
 */
public class UdpService {

    public static void initialize(String alertServiceHost, int alertServicePort, String telemetryServiceHost, int telemetryServicePort)
    {
    	UdpService._alertServiceHost = alertServiceHost; 
    	UdpService._alertServicePort = alertServicePort;
    	UdpService._telemetryServiceHost = telemetryServiceHost; 
    	UdpService._telemetryServicePort = telemetryServicePort;
    }
    
    private static void initializeAlertService()
    {
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

    private static void initializeTelemetryService()
    {
    	try
    	{
            _telemetryServerAddress = InetAddress.getByName(_telemetryServiceHost);
            _telemetrySocket = new DatagramSocket();
            _telemetrySocket.setBroadcast(false);
            _telemetrySocket.setReuseAddress(true);
    	}
    	catch (Exception e)
        {
            _telemetrySocket = null;
        }
    }
    
    private static InetAddress _alertServerAddress, _telemetryServerAddress;
    private static int _alertServicePort, _telemetryServicePort;
    private static DatagramSocket _alertSocket = null, _telemetrySocket = null;
    private static String _alertServiceHost, _telemetryServiceHost;
    
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
    
    public static void sendTelemetry(TelemetryMessage telemetryMessage)
    {
    	if (_telemetrySocket == null) initializeTelemetryService();
    	if (_telemetrySocket == null) return;
        byte[] telemetryBytes = telemetryMessage.toBytes();
        DatagramPacket packet = new DatagramPacket(telemetryBytes, telemetryBytes.length, _telemetryServerAddress, _telemetryServicePort);
        //System.out.println("Sending " + telemetryBytes.length + " bytes of telemetry data");
        try {
            _telemetrySocket.send(packet);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
    
    public static void close()
    {
        try
        {
            if (_alertSocket != null) _alertSocket.close();
            if (_telemetrySocket != null) _telemetrySocket.close();
        }
        catch (Exception e) {}
    }

    /**
     * @param args the command line arguments
     */
    public static void main(String[] args) {
        // TODO code application logic here

        try {
            int port = 5810;

            // Create a socket to listen on the port.
            DatagramSocket dsocket = new DatagramSocket(port);

            // Create a buffer to read datagrams into. If a
            // packet is larger than this buffer, the
            // excess will simply be discarded!
            byte[] buffer = new byte[2048];

            // Create a packet to receive data into the buffer
            DatagramPacket packet = new DatagramPacket(buffer, buffer.length);
            System.out.println("Waiting for tote detection message");
            // Now loop forever, waiting to receive packets and printing them.
            while (true) {
              // Wait to receive a datagram
              dsocket.receive(packet);

              // Convert the contents to a string, and display them
              String msg = new String(buffer, 0, packet.getLength());
              System.out.println(packet.getAddress().getHostName() + ": "
                  + msg);

              if (msg != "none") {
                List<String> fields = Arrays.asList(msg.split(","));
                System.out.println("Tote detected at X = " + fields.get(0) + ", Y = " + fields.get(1) + ", Theta = " + fields.get(2));
              } else {
                  System.out.println("No totes detected");
              }
              // Reset the length of the packet before reusing it.
              packet.setLength(buffer.length);
            }
        } catch (Exception e) {
            System.err.println(e);
        }
        System.out.println("Testing UdpAlertService");
        try
        {
            System.out.println("Setting alert server to 127.0.0.1:5801 and telemetry server to 127.0.0.1:5802");
            UdpService.initialize("127.0.0.1", 5801, "127.0.0.1", 5802);
            if (false)
            {
                System.out.println("Sending alert messages");
                UdpService.sendAlert(new AlertMessage("Robot Ready 1").severity(AlertMessage.Severity.DEBUG).playSound("startup2.wav"));
                UdpService.sendAlert(new AlertMessage("Robot Ready 2 (not filtered due to ERROR severity)").severity(AlertMessage.Severity.ERROR).playSound("startup2.wav"));
                UdpService.sendAlert(new AlertMessage("Robot Ready 1").severity(AlertMessage.Severity.DEBUG).playSound("startup2.wav"));
                UdpService.sendAlert(new AlertMessage("Robot Ready 2 (not filtered due to ERROR severity)").severity(AlertMessage.Severity.ERROR).playSound("startup2.wav"));
                UdpService.sendAlert(new AlertMessage("Robot Ready 1").severity(AlertMessage.Severity.DEBUG).playSound("startup2.wav"));
                UdpService.sendAlert(new AlertMessage("Robot Ready 2 (not filtered due to ERROR severity)").severity(AlertMessage.Severity.ERROR).playSound("startup2.wav"));
                UdpService.sendAlert(new AlertMessage("Robot Ready 3").severity(AlertMessage.Severity.DEBUG).playSound("startup2.wav"));
                Thread.sleep(500);
                UdpService.sendAlert(new AlertMessage("Chitchat Alert").severity(AlertMessage.Severity.CHITCHAT));
                Thread.sleep(500);
                UdpService.sendAlert(new AlertMessage("Debug Alert").severity(AlertMessage.Severity.DEBUG));
                Thread.sleep(500);
                UdpService.sendAlert(new AlertMessage("Warning Alert (with Sound that is available)").severity(AlertMessage.Severity.WARNING).playSound("boing.wav"));
                Thread.sleep(500);
                UdpService.sendAlert(new AlertMessage("Warning Alert (with sound that is not available)").severity(AlertMessage.Severity.WARNING).playSound("missing.wav"));
                Thread.sleep(500);
                UdpService.sendAlert(new AlertMessage("Error Alert").severity(AlertMessage.Severity.ERROR));
                Thread.sleep(500);
                UdpService.sendAlert(new AlertMessage("Fatal Alert (with cool WALL-E sound)").playSound("roguerobots.wav").severity(AlertMessage.Severity.FATAL));
                Thread.sleep(500);
            }
            System.out.println("Sending telemetry...");
            TelemetryMessage telemetry = new TelemetryMessage();
            double shoulderStep = 1, elbowStep = 2, wristStep = 1;
            telemetry.shoulderJointAngle = -45;
            telemetry.elbowJointAngle = -90;
            telemetry.wristJointAngle = 0;
            try
            {
                while (true)
                {
                    telemetry.shoulderJointAngle += shoulderStep;
                    telemetry.elbowJointAngle += elbowStep;
                    telemetry.wristJointAngle += wristStep;
                    UdpService.sendTelemetry(telemetry);
                    if (telemetry.shoulderJointAngle <= -45 || telemetry.shoulderJointAngle >= 135) shoulderStep *= -1;
                    if (telemetry.elbowJointAngle <= -90 || telemetry.elbowJointAngle >= 135) elbowStep *= -1;
                    if (telemetry.wristJointAngle <= 0 || telemetry.wristJointAngle >= 180) wristStep *= -1;
                    Thread.sleep(20);
                }
            }
            catch (Exception e) 
            {
                e.printStackTrace();
            }
            System.out.println("Closing connection to alert server");
            UdpService.close();
        }
        catch (Exception e)
        {
            System.out.println("Caught exception: " + e.getMessage());
        }
    }
}
