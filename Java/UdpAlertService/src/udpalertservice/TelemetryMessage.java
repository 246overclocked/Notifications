/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package udpalertservice;

import java.nio.ByteBuffer;
import java.nio.ByteOrder;
/**
 *
 * @author Dave Anderson
 */
public class TelemetryMessage {
    // TODO: Make sure to increment this number every time the structure changes
    //       This will allow the receiver code to know if it's decoding the right
    //       message. If it tries to decode the wrong message, the posture monitor
    //       and everything else that may potentially use the telemetry will
    //       be working with potentially invalid data.  As the saying goes,
    //       Garbage In, Garbage Out.
    private static int telemetryVersionNumber = 2; 
    public double frontLeftWheelCommandedSpeed;         //  1
    public double frontLeftWheelActualSpeed;            //  2
    public double frontLeftWheelSteeringSpeed;          //  3
    public double frontLeftWheelActualAngle;            //  4
    public double frontRightWheelCommandedSpeed;        //  5
    public double frontRightWheelActualSpeed;           //  6
    public double frontRightWheelSteeringSpeed;         //  7
    public double frontRightWheelActualAngle;           //  8
    public double rearWheelCommandedSpeed;              //  9
    public double rearWheelActualSpeed;                 // 10
    public double rearWheelSteeringSpeed;               // 11
    public double rearWheelActualAngle;                 // 12
    public double shoulderJointSpeed;                   // 13
    public double shoulderJointAngle;                   // 14
    public double elbowJointSpeed;                      // 15
    public double elbowJointAngle;                      // 16
    public double wristJointSpeed;                      // 17
    public double wristJointAngle;                      // 18
    public double grabberCommandedSpeed;                // 19
    public double grabberTickCount;                     // 20
    public double liftCommandedSpeed;                   // 21
    public double liftActualHeight;                     // 22
    public double leftRangeSensorValue;                 // 23
    public double rightRangeSensorValue;                // 24
    public double leftGetterCommandedSpeed;             // 25
    public double rightGetterCommandedSpeed;            // 26
    public double navxPitch;                            // 27
    public double navxRoll;                             // 28
    public double navxYaw;                              // 29
    public double navxXAccel;                           // 30
    public double navxYAccel;                           // 31
    public double navxZAccel;                           // 32
    public double navxQuaternionW;                      // 33
    public double navxQuaternionX;                      // 34
    public double navxQuaternionY;                      // 35
    public double navxQuaternionZ;                      // 36
    
    public byte elbowWristAngleConstraintViolation;     //  1
    public byte forearmLiftConstraintViolation;         //  2
    public byte grabberCeilingConstraintViolation;      //  3
    public byte grabberGroundConstraintViolation;       //  4
    public byte grabberLiftConstraintViolation;         //  5
    public byte shoulderAngleConstraintViolation;       //  6
    public byte shoulderElbowAngleConstraintViolation;  //  7
    public byte shoulderWristAngleConstraintViolation;  //  8
    public byte wristCeilingConstraintViolation;        //  9
    public byte wristGroundConstraintViolation;         // 10

    // TODO: Keep these updated with the number of member variables of the respective types that
    //       will be wrapped in the byte array
    private static final int DOUBLE_MEMBER_COUNT = 36;
    private static final int LONG_MEMBER_COUNT = 0;
    private static final int FLOAT_MEMBER_COUNT = 0;
    private static final int INT_MEMBER_COUNT = 0;
    private static final int SHORT_MEMBER_COUNT = 0;
    private static final int BYTE_MEMBER_COUNT = 10;
    
    private static final int DOUBLE_SIZE = 8;
    private static final int LONG_SIZE = 8;
    private static final int FLOAT_SIZE = 4;
    private static final int INT_SIZE = 4;
    private static final int SHORT_SIZE = 2;
    private static final int BYTE_SIZE = 1;
    
    public byte[] toBytes()
    {
        // TODO: Make SURE that if you change any of the class member variables that need to be transmitted over
        // a network connection that you make corresponding changes HERE
        int sizeInBytes = (DOUBLE_MEMBER_COUNT * DOUBLE_SIZE) + (LONG_MEMBER_COUNT * LONG_SIZE) + 
                          (FLOAT_MEMBER_COUNT * FLOAT_SIZE) + (INT_MEMBER_COUNT * INT_SIZE) +
                          (SHORT_MEMBER_COUNT * SHORT_SIZE) + (BYTE_MEMBER_COUNT * BYTE_SIZE);
        _bytes = new byte[sizeInBytes + INT_SIZE];
        ByteBuffer bb = ByteBuffer.wrap(_bytes);
        bb.order(ByteOrder.LITTLE_ENDIAN);
        int byteIndex = 0;
        // NOTE: This field MUST be first
        bb.putInt(byteIndex, telemetryVersionNumber);
        byteIndex += INT_SIZE;
        // Telemetry data starts here
        bb.putDouble(byteIndex, frontLeftWheelCommandedSpeed);      //  1
        byteIndex += DOUBLE_SIZE;
        bb.putDouble(byteIndex, frontLeftWheelActualSpeed);         //  2
        byteIndex += DOUBLE_SIZE;
        bb.putDouble(byteIndex, frontLeftWheelSteeringSpeed);       //  3
        byteIndex += DOUBLE_SIZE;
        bb.putDouble(byteIndex, frontLeftWheelActualAngle);         //  4
        byteIndex += DOUBLE_SIZE;
        bb.putDouble(byteIndex, frontRightWheelCommandedSpeed);     //  5
        byteIndex += DOUBLE_SIZE;
        bb.putDouble(byteIndex, frontRightWheelActualSpeed);        //  6
        byteIndex += DOUBLE_SIZE;
        bb.putDouble(byteIndex, frontRightWheelSteeringSpeed);      //  7
        byteIndex += DOUBLE_SIZE;
        bb.putDouble(byteIndex, frontRightWheelActualAngle);        //  8
        byteIndex += DOUBLE_SIZE;
        bb.putDouble(byteIndex, rearWheelCommandedSpeed);           //  9
        byteIndex += DOUBLE_SIZE;
        bb.putDouble(byteIndex, rearWheelActualSpeed);              // 10
        byteIndex += DOUBLE_SIZE;
        bb.putDouble(byteIndex, rearWheelSteeringSpeed);            // 11
        byteIndex += DOUBLE_SIZE;
        bb.putDouble(byteIndex, rearWheelActualAngle);              // 12
        byteIndex += DOUBLE_SIZE;
        bb.putDouble(byteIndex, shoulderJointSpeed);                // 13
        byteIndex += DOUBLE_SIZE;
        bb.putDouble(byteIndex, shoulderJointAngle);                // 14
        byteIndex += DOUBLE_SIZE;
        bb.putDouble(byteIndex, elbowJointSpeed);                   // 15
        byteIndex += DOUBLE_SIZE;
        bb.putDouble(byteIndex, elbowJointAngle);                   // 16
        byteIndex += DOUBLE_SIZE;
        bb.putDouble(byteIndex, wristJointSpeed);                   // 17
        byteIndex += DOUBLE_SIZE;
        bb.putDouble(byteIndex, wristJointAngle);                   // 18
        byteIndex += DOUBLE_SIZE;
        bb.putDouble(byteIndex, grabberCommandedSpeed);             // 19
        byteIndex += DOUBLE_SIZE;
        bb.putDouble(byteIndex, grabberTickCount);                  // 20
        byteIndex += DOUBLE_SIZE;
        bb.putDouble(byteIndex, liftCommandedSpeed);                // 21
        byteIndex += DOUBLE_SIZE;
        bb.putDouble(byteIndex, liftActualHeight);                  // 22
        byteIndex += DOUBLE_SIZE;
        bb.putDouble(byteIndex, leftRangeSensorValue);              // 23
        byteIndex += DOUBLE_SIZE;
        bb.putDouble(byteIndex, rightRangeSensorValue);             // 24
        byteIndex += DOUBLE_SIZE;
        bb.putDouble(byteIndex, leftGetterCommandedSpeed);          // 25
        byteIndex += DOUBLE_SIZE;
        bb.putDouble(byteIndex, rightGetterCommandedSpeed);         // 26
        byteIndex += DOUBLE_SIZE;
        bb.putDouble(byteIndex, navxPitch);                         // 27
        byteIndex += DOUBLE_SIZE;
        bb.putDouble(byteIndex, navxRoll);                          // 28
        byteIndex += DOUBLE_SIZE;
        bb.putDouble(byteIndex, navxYaw);                           // 29
        byteIndex += DOUBLE_SIZE;
        bb.putDouble(byteIndex, navxXAccel);                        // 30
        byteIndex += DOUBLE_SIZE;
        bb.putDouble(byteIndex, navxYAccel);                        // 31
        byteIndex += DOUBLE_SIZE;
        bb.putDouble(byteIndex, navxZAccel);                        // 32
        byteIndex += DOUBLE_SIZE;
        bb.putDouble(byteIndex, navxQuaternionW);                   // 33
        byteIndex += DOUBLE_SIZE;
        bb.putDouble(byteIndex, navxQuaternionX);                   // 34
        byteIndex += DOUBLE_SIZE;
        bb.putDouble(byteIndex, navxQuaternionY);                   // 35
        byteIndex += DOUBLE_SIZE;
        bb.putDouble(byteIndex, navxQuaternionZ);                   // 36
        byteIndex += DOUBLE_SIZE;
        
        // Bytes
        bb.put(byteIndex, elbowWristAngleConstraintViolation);      //  1
        byteIndex += BYTE_SIZE;
        bb.put(byteIndex, forearmLiftConstraintViolation);          //  2
        byteIndex += BYTE_SIZE;
        bb.put(byteIndex, grabberCeilingConstraintViolation);       //  3
        byteIndex += BYTE_SIZE;
        bb.put(byteIndex, grabberGroundConstraintViolation);        //  4
        byteIndex += BYTE_SIZE;
        bb.put(byteIndex, grabberLiftConstraintViolation);          //  5
        byteIndex += BYTE_SIZE;
        bb.put(byteIndex, shoulderAngleConstraintViolation);        //  6
        byteIndex += BYTE_SIZE;
        bb.put(byteIndex, shoulderElbowAngleConstraintViolation);   //  7
        byteIndex += BYTE_SIZE;
        bb.put(byteIndex, shoulderWristAngleConstraintViolation);   //  8
        byteIndex += BYTE_SIZE;
        bb.put(byteIndex, wristCeilingConstraintViolation);         //  9
        byteIndex += BYTE_SIZE;
        bb.put(byteIndex, wristGroundConstraintViolation);          // 10
        byteIndex += BYTE_SIZE;
        return _bytes;
    }
    private byte[] _bytes;
}
