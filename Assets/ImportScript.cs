using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

/*
 * v0.1 2015/09/13
 *  - checked with udpTimeGraph import
 */ 

public class ImportScript : MonoBehaviour {
	
	public InputField IFipadr;
	public InputField IFport;
	public Text rcvText; // recieved text
	public string sendCommand;

	int getPort() {
		int res = Convert.ToInt16 (IFport.text);
		if (res < 0) {
			res = 0;
		}
		return res;
	}

	bool SendCommand(ref UdpClient client, string ipadr, int port) {
		string sendstr = sendCommand + System.Environment.NewLine;
		byte[] data = ASCIIEncoding.ASCII.GetBytes (sendstr);

		try {
			client.Send (data, data.Length, ipadr, port);
		}
		catch (Exception e) {
			rcvText.text = "snd:" + e.Message;
			return false;
		}
		return true;
	}
	
	void procComm() {
		UdpClient client = new UdpClient ();
		client.Client.SendTimeout = 1000; // msec
		client.Client.ReceiveTimeout = 2000; // msec

		if (SendCommand (ref client, (IFipadr.text), getPort()) == false) {
			return;
		}

		// receive
		IPEndPoint remoteIP = new IPEndPoint(IPAddress.Any, 0);
		string rcvdstr = "";
		byte [] data;

		while (true) {
			try {
				data = client.Receive (ref remoteIP);
				if (data.Length == 0) {
					break; // no response
				}
				string text = Encoding.ASCII.GetString (data);
				rcvdstr += text;
				if (text.Contains("EOT")) { // End of Table
					break;
				}
			} catch (Exception err) {
				Debug.Log(err.Message);
				rcvText.text = "no response";
				break;
			}
		}
		
		client.Close ();

		if (rcvdstr.Length > 0) {
			System.IO.File.WriteAllText("import.csv", rcvdstr);
			rcvText.text = "recvd to csv";
		}
	}
	
	public void onClick() {
		procComm ();
	}
}
