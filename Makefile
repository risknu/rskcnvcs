CC = g++
CFLAGS = -std=c++11 -Wall -Wextra
TARGET = bin/server
SOURCE = src/cpp/server.cpp

all: $(TARGET)

$(TARGET): $(SOURCE)
	$(CC) -o $(TARGET) $(SOURCE) $(CFLAGS)

clean:
	rm -f $(TARGET)
