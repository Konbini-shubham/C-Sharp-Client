using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;
using MachineDll;
using WebSocketSharp;
using System.Runtime.InteropServices;
namespace Test
{
	public partial class Form1 : Form
	{
        const string URL_CONNECTION_WEBSOCKET = "ws://localhost:8888";
        private StatusCapture machine;

		private int coin1;
		private int coin2;
        WebSocket ws;

        private void communicateWithBackend()
        {
            
            ws.OnOpen += (sender, e) =>
            {
                Console.WriteLine("OnOpen");
            };

            ws.OnMessage += (sender, e) =>
            {
                Console.WriteLine("Laputa says: " + e.Data);
                if (e.IsPing)
                {
                    Console.WriteLine("Ping received.");
                }
                else if (e.IsText)
                {
                    Console.WriteLine("Message of type text has been received.");
                }

                else if (e.IsBinary)
                {
                    Console.WriteLine("Message of type binary has been replaced.");
                }
            };

            ws.OnError += (sender, e) => {
                Console.WriteLine("Error happened.");
                Console.WriteLine("Error message: " + e.Message);
            };

            ws.OnClose += (sender, e) => {
                Console.WriteLine("OnClose");
                Console.WriteLine("Status Code: " + e.Code);
                Console.WriteLine("Reason for close: " + e.Reason);
            };
            ws.EmitOnPing = true;
            ws.Connect();
            ws.Send("BALUS");
            Console.WriteLine("I am here");
        }

        public Form1()
		{
			InitializeComponent();

			comboBox1.SelectedIndex = 0;
			timer1.Enabled = true;
            ws = new WebSocket(URL_CONNECTION_WEBSOCKET);
            communicateWithBackend();
		}

    ~Form1()
		{
			machine.Close();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			machine.Close();
			base.OnClosing(e);
		}

		private void machine_OnEventStatus(int packageCode, object obj)
		{
			if (packageCode == 10)
			{
				MessageBox.Show(((SCSale)obj).param2.ToString());
			}

			//if (packageCode == 14)
			//{
			//    machine.Coin_Confirm(true);
			//}

			//if (packageCode == 15)
			//{
			//    //timer2.Enabled = true;
			//    MessageBox.Show(machine.Coin_Acceptor(0, 0).ToString());
			//}

			string val = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t" + packageCode + ":	" + obj.ToString() + "\r\n";

			listBox1.Items.Add(val);
			listBox1.SelectedIndex = listBox1.Items.Count - 1;
			File.AppendAllText(Application.StartupPath + "\\Log.txt", val);
		}

		private void machine_OnEventError(int etype, string message)
		{
			string val = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\tERROR       " + etype + ":	" + message + "\r\n";

			listBox1.Items.Add(val);
			listBox1.SelectedIndex = listBox1.Items.Count - 1;
			File.AppendAllText(Application.StartupPath + "\\Log.txt", val);
		}

		private void button3_Click(object sender, EventArgs e)
		{
			machine.VMC_Test("单道", textBox2.Text);
		}

		private void button2_Click(object sender, EventArgs e)
		{
			machine.VMC_Test("全道", "");
		}

		private void button4_Click(object sender, EventArgs e)
		{
			if (machine == null)
			{
				machine = new StatusCapture();
				machine.OnEventError += new MachineDll.StatusCaptureErrorEventHandler(machine_OnEventError);
				machine.OnEventStatus += new MachineDll.StatusCaptureEventHandler(machine_OnEventStatus);
				machine.Start((ECOM)Enum.Parse(typeof(ECOM), comboBox1.SelectedItem.ToString()), 10);
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			StringBuilder val = new StringBuilder();

			foreach (string item in listBox1.Items)
			{
				val.Append(item + "\r\n");
			}

			File.AppendAllText(Application.StartupPath + "\\" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".log", val.ToString());

			machine.Close();
			machine = null;
		}

		private void button5_Click(object sender, EventArgs e)
		{
			machine.VMC_Status();
		}

		private void button6_Click(object sender, EventArgs e)
		{
			coin1 = 1;
			MessageBox.Show(machine.Coin_Acceptor(coin1, coin2).ToString());
		}

		private void button7_Click(object sender, EventArgs e)
		{
			coin1 = 0;
			MessageBox.Show(machine.Coin_Acceptor(coin1, coin2).ToString());
		}

		private void button8_Click(object sender, EventArgs e)
		{
			coin2 = 1;
			MessageBox.Show(machine.Coin_Acceptor(coin1, coin2).ToString());
		}

		private void button9_Click(object sender, EventArgs e)
		{
			coin2 = 0;
			MessageBox.Show(machine.Coin_Acceptor(coin1, coin2).ToString());
		}

		private void button10_Click(object sender, EventArgs e)
		{
			MessageBox.Show(machine.Coin_Init().ToString());
		}

		private void button12_Click(object sender, EventArgs e)
		{
			machine.Coin_Confirm(true);
		}

		private void button17_Click(object sender, EventArgs e)
		{
			machine.Coin_Confirm(false);
		}

		private void button13_Click(object sender, EventArgs e)
		{
			machine.Coin_Return(int.Parse(textBox1.Text));
		}

		private void button14_Click(object sender, EventArgs e)
		{
			machine.VMC_Sale(textBox3.Text);
		}

		private void button11_Click(object sender, EventArgs e)
		{
			machine.Coin_Status();
		}

		private void button15_Click(object sender, EventArgs e)
		{
			machine.VMC_Test("升降", textBox4.Text);
		}

		private bool m_pick = false;

		private void button16_Click(object sender, EventArgs e)
		{
			if (!m_pick)
			{
				machine.VMC_Test("红外", "");
			}
			else
			{
				machine.VMC_ExitTest();
			}

			m_pick = !m_pick;
		}

		private void button18_Click(object sender, EventArgs e)
		{
			machine.Coin_Qty();
		}

		private void button20_Click(object sender, EventArgs e)
		{
			machine.Elevator_Location();
		}

		private void timer2_Tick(object sender, EventArgs e)
		{
			timer2.Enabled = false;
			MessageBox.Show(machine.Coin_Acceptor(0, 0).ToString());
		}

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}