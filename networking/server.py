import socket
import threading

clients = []

def handle_client(client_socket, client_address):
    print(f"New Connection {client_address}")
    clients.append(client_socket)
    
    try:
        while True:
            message = client_socket.recv(1024)
            if not message:
                break
            print('message')
            for client in clients:
                if client != client_socket:
                    client.sendall(message)
    finally:
        print(f"Disconnect {client_address}")
        clients.remove(client_socket)
        client_socket.close()

def main():
    server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server.bind(("127.0.0.1", 2425))
    server.listen(4)
    print("Server started...")

    try:
        while True:
            client_socket, client_address = server.accept()
            client_handler = threading.Thread(target=handle_client, args=(client_socket, client_address))
            client_handler.start()
    finally:
        server.close()

if __name__ == "__main__":
    main()
