CC = g++
CFLAGS = -std=c++11
TARGET = bin/server
SOURCE = src/cpp/server.cpp

all: $(TARGET)

$(TARGET): $(SOURCE)
	$(CC) $(CFLAGS) -o $(TARGET) $(SOURCE)

clean:
	rm -f $(TARGET)
