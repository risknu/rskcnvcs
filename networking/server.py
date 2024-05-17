from __future__ import annotations

# TODO: add logs

from typing import Optional, Any
import socket
import threading
import argparse
import signal
import sys

parser = argparse.ArgumentParser(
    prog='rskcnvcs<server.py>',
    description='Project rskcnvcs server script: server.py script for starting the server',
    epilog='More information can be found on the project\'s GitHub page')
parser.add_argument('-i', '--ipvf', type=str, default="127.0.0.1",
                    help="Server IP version 4 for example \"127.0.0.1\"")
parser.add_argument('-p', '--port', type=int, default=2425,
                    help="Server port for example \"2425\"")

args: argparse.Namespace = parser.parse_args()

clients_list: list[socket.socket] = []

def handle_client(client_socket: Optional[socket.socket] = None, client_addres: Optional[tuple[str, int]] = None) -> None:
    print('[*] New connection registered in the registration list')
    clients_list.append(client_socket)
    try:
        while True:
            client_message: bytes = client_socket.recv(2048)
            if client_message is None:
                break
            for client in clients_list:
                if client != client_socket:
                    client.sendall(client_message)
    except Exception as e:
        print(f'[!] ERR: {e}; client break; continue program')
        print('[*][1] Client disconnected, removing from the registration list')
        clients_list.remove(client)
        client_socket.close()
    finally:
        print('[*][0] Client disconnected, removing from the registration list')
        clients_list.remove(client)
        client_socket.close()
        
def signal_handler(sig: Any = None, frame: Any = None) -> None:
    print('[*] Server shutting down gracefully...')
    sys.exit(0)

def main() -> None:
    signal.signal(signal.SIGINT, signal_handler)
    
    server: socket.socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server.bind((args.ipvf, args.port))
    server.listen(4)
    print(f"[*] Server started, listening for connections at address {args.ipvf}:{args.port}")
    try:
        while True:
            client_socket, client_address = server.accept()
            client_handler = threading.Thread(target=handle_client, args=(client_socket, client_address))
            client_handler.start()
    except Exception as e:
        print(f'[!] ERR: {e}; continue program')
    finally:
        server.close()
        
if __name__ == "__main__":
    main()
        