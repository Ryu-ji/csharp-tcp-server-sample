private TcpListener tcpListener;

        private int bsize = 1024;

        public Server() { }

        public Server(int bufferSize) { if (bufferSize == 0) { bsize = 1024; } }

        public void Listen(string ip, int port)
        {
            tcpListener = new TcpListener(IPAddress.Parse(ip), port);

            tcpListener.Start();
        }

        public async Task<TcpClient> AcceptAsync() => await tcpListener.AcceptTcpClientAsync();


        public async Task<(NetworkStream, byte[])> ReceiveAsync(TcpClient tcpClient)
        {
            NetworkStream networkStream = tcpClient.GetStream();
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    byte[] buffer = new byte[bsize];

                    do
                    {
                        int size = await networkStream.ReadAsync(buffer, 0, buffer.Length);

                        await memoryStream.WriteAsync(buffer, 0, size);

                    } while (networkStream.DataAvailable);

                    return (networkStream, memoryStream.GetBuffer());
                }
            }

        }


        public async Task SendAsync(NetworkStream networkStream, byte[] msgBytes)
        {
            await networkStream.WriteAsync(msgBytes, 0, msgBytes.Length);
        }



        //TCP Listenそのものを停止
        public void StopListen()
        {
            if (tcpListener != null)
                tcpListener.Stop();
        }
