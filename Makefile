CC = g++
CFLAGS = -std=c++11 -Wall -Wextra
TARGET = bin/server
SOURCE = server/server.cpp server/logger.cpp server/utils/fileutil.cpp

all: $(TARGET)

$(TARGET): $(SOURCE)
	$(CC) -o $(TARGET) $(SOURCE) $(CFLAGS)

clean:
	rm -f $(TARGET)
