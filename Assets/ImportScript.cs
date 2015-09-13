using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class ImportScript : MonoBehaviour {
	
	public InputField IFipadr;
	public InputField IFport;
	public InputField IFmsg; // message to send
	public Text rcvText; // recieved text
	
	UdpClient client;
	int port;
	string lastRcvd;
	
	void Start() {
		IFipadr.text = "192.168.10.";
		IFport.text = "6000";
	}
	
	void Update() {
		rcvText.text = lastRcvd;
	}
	
	string getIpadr() {
		return IFipadr.text;
	}
	int getPort() {
		int res = Convert.ToInt16 (IFport.text);
		if (res < 0) {
			res = 0;
		}
		return res;
	}
	
	void procComm() {
		port = getPort();
		string ipadr = getIpadr ();
		
		client = new UdpClient ();
		
		// send
		string sendstr = IFmsg.text + System.Environment.NewLine;
		byte[] data = ASCIIEncoding.ASCII.GetBytes (sendstr);
		client.Send (data, data.Length, ipadr, port);
		
		// receive
		client.Client.ReceiveTimeout = 2000; // msec
		IPEndPoint remoteIP = new IPEndPoint(IPAddress.Any, 0);
		lastRcvd = "";
		try {
			data = client.Receive (ref remoteIP);
			if (data.Length > 0) {
				string text = Encoding.ASCII.GetString (data);
				lastRcvd = text;	
			}
		} catch (Exception err) {
		}
		
		client.Close ();
	}
	
	public void onClick() {
		Debug.Log ("on click");
		procComm ();
	}
}
