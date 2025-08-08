const { HubConnectionBuilder, HttpTransportType, LogLevel } = require('@microsoft/signalr')

;(async () => {
  // 1) Build the connection (match your server’s URL + port)
  const connection = new HubConnectionBuilder()
    .withUrl('http://localhost:5164/chat', {
      skipNegotiation: true,
      transport: HttpTransportType.WebSockets
    })
    .configureLogging(LogLevel.Information)
    .build()

  // 2) Register the ReceiveMessage handler *before* starting the connection
  connection.on('ReceiveMessage', (sender, text) => {
    if (sender === 'AI') {
      console.log(`"AI-engine" ${text}`)
    } else {
      console.log(`"User:${sender}" ${text}`)
    }
  })

  // 3) Start the connection
  try {
    await connection.start()
    console.log('Connected to ChatHub')
  } catch (err) {
    console.error('Failed to connect:', err)
    return
  }

  // 4) Invoke your hub method with the correct parameters
  const myUserName = 'TestUser'
  const myMessage = 'Hello from test client!!'

  console.log(`Sending message as ${myUserName}: "${myMessage}"`)
  try {
    await connection.invoke('SendMessage', myUserName, myMessage)
  } catch (err) {
    console.error('Failed to invoke SendMessage:', err)
  }
})()
