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
	public Text rcvText; // recieved text
	public string sendCommand;
	
	UdpClient client;
	int port;
	string lastRcvd;
	
	void Start() {
	}
	
	void Update() {
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
		string sendstr = sendCommand + System.Environment.NewLine;
		byte[] data = ASCIIEncoding.ASCII.GetBytes (sendstr);
		client.Client.SendTimeout = 1000; // msec

		try {
			client.Send (data, data.Length, ipadr, port);
		}
		catch (Exception e) {
			Debug.Log(e.Message);
			return;
		}

		// receive
		client.Client.ReceiveTimeout = 2000; // msec
		IPEndPoint remoteIP = new IPEndPoint(IPAddress.Any, 0);
		lastRcvd = "";

		while (true) {
			try {
				data = client.Receive (ref remoteIP);
				if (data.Length == 0) {
					Debug.Log("no response");
					return;
				}
				string text = Encoding.ASCII.GetString (data);
				lastRcvd += text;

				if (text.Contains("EOT")) {
					break;
				}
			} catch (Exception err) {
				Debug.Log(err.Message);
				rcvText.text = "no response";
				break;
			}
		}
		
		client.Close ();

		if (lastRcvd.Length > 0) {
			System.IO.File.WriteAllText("import.csv", lastRcvd);
			rcvText.text = "recvd to csv";
		}
	}
	
	public void onClick() {
		procComm ();
	}
}
