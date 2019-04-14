using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LockVsAwait
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            var x = ThreadPool.SetMaxThreads(10, 10);
        }
        int _sharedVariable;
        private readonly object _objLock = new object();
        int workerThreads = 0;
        int complitionPortThreads = 0;

        private  async void button1_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            listView2.Items.Clear();

            listView2.Items.Add("Before creating T1 and T2");

            listView2.Items.Add($"Worker Threads:{workerThreads}, Complition Port Threads:{complitionPortThreads}");

            Task t1 = new Task(() => ProcessItem(()=> UpdateUIThread()));
            t1.Start();
            Task t2 = new Task(() => ProcessItem(() => UpdateUIThread()));
            t2.Start();

            ThreadPool.GetAvailableThreads(out workerThreads, out complitionPortThreads);
            listView2.Items.Add("After creating T1 and T2");
            listView2.Items.Add($"Worker Threads:{workerThreads}, Complition Port Threads:{complitionPortThreads}");

            listView2.Items.Add("Waiting for T1 and T2");

            await t1;
            await t2;
           

            ThreadPool.GetAvailableThreads(out workerThreads, out complitionPortThreads);
            listView2.Items.Add("After T1 and T2 finishes");

            listView2.Items.Add($"Worker Threads:{workerThreads}, Complition Port Threads:{complitionPortThreads}");

        }
        private void UpdateUIThread()
        {
            this.listView1.Invoke((MethodInvoker)delegate
            {
                listView1.Items.Add(_sharedVariable.ToString());
            });
        }

        private void ProcessItem(Action updateUIThread)
        {
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(10);
                lock (_objLock)
                {
                    _sharedVariable++;
                    updateUIThread();
                }
            }
        }
        private void ProcessItem()
        {
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(10);
                lock (_objLock)
                {
                    _sharedVariable++;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            listView2.Items.Clear();
            listView2.Items.Add("Before creating T1 and T2");

            listView2.Items.Add($"Worker Threads:{workerThreads}, Complition Port Threads:{complitionPortThreads}");

            Thread t1 = new Thread(new ThreadStart(ProcessItem));
            Thread t2 = new Thread(new ThreadStart(ProcessItem));

           

            ThreadPool.GetAvailableThreads(out workerThreads, out complitionPortThreads);
            listView2.Items.Add("After creating T1 and T2");

            listView2.Items.Add($"Worker Threads:{workerThreads}, Complition Port Threads:{complitionPortThreads}");
            listView2.Items.Add("Waiting for T1 and T2");

          
            t1.Start();
            listView2.Items.Add("After runing T1 ");
            listView2.Items.Add($"Worker Threads:{workerThreads}, Complition Port Threads:{complitionPortThreads}");
            t1.Join();
            listView2.Items.Add("After runing T2 ");
            listView2.Items.Add($"Worker Threads:{workerThreads}, Complition Port Threads:{complitionPortThreads}");
            t2.Start();
            t2.Join();

            ThreadPool.GetAvailableThreads(out workerThreads, out complitionPortThreads);
            listView2.Items.Add("After T1 and T2 finishes");

            listView2.Items.Add($"Worker Threads:{workerThreads}, Complition Port Threads:{complitionPortThreads}");
            UpdateUIThread();
        }
    }
}
