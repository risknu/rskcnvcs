import socket
import threading

# Server configuration
HOST = 'localhost'
PORT = 5555

# Create socket
server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_socket.bind((HOST, PORT))
server_socket.listen()

# List to store connected clients
clients = []

def handle_client(client_socket: socket.socket, addr):
    while True:
        try:
            data = client_socket.recv(1024)
            if not data:
                break
            for c in clients:
                if c != client_socket:
                    c.send(data)    
        except:
            break
    clients.remove(client_socket)
    client_socket.close()

# Accept and handle multiple clients
while True:
    client_socket, addr = server_socket.accept()
    clients.append(client_socket)

    # Start a new thread for each connected client
    client_thread = threading.Thread(target=handle_client, args=(client_socket, addr))
    client_thread.start()
