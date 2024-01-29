import pygame
import socket
import pickle
from threading import Thread

# Client configuration
SERVER_HOST = '127.0.0.1'
SERVER_PORT = 5555

# Initialize Pygame
pygame.init()

# Set up the screen
width, height = 720, 440
screen = pygame.display.set_mode((width, height))
pygame.display.set_caption("Pixel Paint Game")

# Colors
WHITE = (255, 255, 255)
BLACK = (0, 0, 0)

# Connect to the server
client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
client_socket.connect((SERVER_HOST, SERVER_PORT))

matrix = [[None for _ in range(width // 10)] for _ in range(height // 10)]
tmp_pixels = []

def receive_data():
    while True:
        try:
            # Receive and unpickle data from the server
            data = client_socket.recv(4096)
            pixels = pickle.loads(data)

            for pixel in pixels:
                matrix[pixel[0]][pixel[1]] = pixel[2]
        except socket.error as e:
            print(f"Error receiving data: {e}")
            break

# Start the thread for receiving data from the server
receive_thread = Thread(target=receive_data)
receive_thread.start()

# Main game loop
drawing = False
color = BLACK
cell_size = 10

while True:
    screen.fill(WHITE)

    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            pygame.quit()
            client_socket.close()
            quit()
        elif event.type == pygame.MOUSEBUTTONDOWN:
            drawing = True
        elif event.type == pygame.MOUSEBUTTONUP:
            drawing = False
            client_socket.send(pickle.dumps(tmp_pixels))
            tmp_pixels = []
        elif event.type == pygame.MOUSEMOTION and drawing:
            x, y = event.pos
            # Snap to the grid
            x = (x // cell_size) * cell_size
            y = (y // cell_size) * cell_size
            pixel = (y // cell_size, x // cell_size, color)  # Fix the order of coordinates
            # Send the pixel data to the server
            tmp_pixels.append(pixel)
            matrix[y // cell_size][x // cell_size] = color

    # Display the matrix on the console
    for row in range(len(matrix)):
        for color in range(len(matrix[row])):
            if matrix[row][color] is not None:
                pygame.draw.rect(screen, matrix[row][color], (color*cell_size, row*cell_size, 10, 10))

    pygame.display.update()
