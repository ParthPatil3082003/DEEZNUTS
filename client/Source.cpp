// client code here

/*
    0x00 : offline
    0x01 : online
    0x02 : tells server to save username of the user
    0x03 : client username is available
    0x04 : clinet username is not available
     .
     .
     .
     -----***** WARNING FOR SRVER *****---------------
     while recieving these signal bytes 
     do not convert recieved bytes to int instead compare them with predefined byte like
     char online = 0x00
     if ( recv_byte == online)
     ---------------------------------------------
*/

#include <stdio.h>
#include <iostream>
#include <thread>
#include <conio.h>
#include <winsock2.h>
#include <WS2tcpip.h>
#include <string>

#pragma comment(lib, "ws2_32.lib")

#define hostname "deeznutserver.centralindia.cloudapp.azure.com"

// initialized neccessary variables 
int loop = 1 , key , bytes_read;
char msgsend[500] , recvbyte;
char msgrecv[500];
std::string msg , username;

// function to send signal to server in byte form 
// takes scoket and required byte( i.e 0x01 etc. ) as input 
void signal( SOCKET sock , char bt) {
    send(sock, &bt, 1, 0);
}

// sends text message to the server
// also returns the message sent in string 
std :: string sendtoserver(SOCKET sock) {
    std::getline(std::cin, msg);
    strncpy_s(msgsend, msg.c_str(), sizeof(msgsend));
    send(sock, msgsend, strlen(msgsend), 0);
    return msg;
}

// sends text message to the server when pressed keys and also terminates the chat 
// press '/' to chat and 'esc' to disconnect
// runs in thread
void sendmsg(SOCKET sock) {

    // loop keeps running until true and stops when condition changed to 0
    while (loop) {
        // waits for the character
        key = _getch();

        // if 'esc' pressed it sets 
        if (key == 27) {
            loop = 0;           //loop = 0 so that while loop of both send and recv function stops
            signal(sock, 0x00); // signals the server that client is offline
            break;              // breaks out of loop and finishes the program
        }
        // if '/' pressed calls the sendtoserver function to send the text message
        else if (key == 47) {
            printf("YOU: ");
            sendtoserver(sock);
        }
    }
}

// this function recievs the chat
// runs in thread
// keeps posting incoming messages
void recvmsg(SOCKET sock , std::string username ) {
    while (loop) {
        bytes_read = recv(sock, msgrecv, 500, 0);
        msgrecv[bytes_read] = '\0';
        printf(username.c_str(), ":", msgrecv, "\n");
    }
}


int main() {
	
	// initializing the socket
    WSAData wsaData;
    int wsaerr = WSAStartup(MAKEWORD(2, 2), &wsaData);
    if (wsaerr != 0) {
        printf("could not initialize wsa\n");
        return 1;
    }

    addrinfo hints;
    addrinfo* result, * ptr;

    ZeroMemory(&hints, sizeof(hints));
    hints.ai_family = AF_INET;
    hints.ai_socktype = SOCK_STREAM;
    hints.ai_protocol = IPPROTO_TCP;

    int getret = getaddrinfo( hostname , "42069", &hints, &result);
    if (getret != 0) {
        printf("getaddrinfo failed\n");
        WSACleanup();
        return 1;
    }

    SOCKET clientsock;

    for (ptr = result; ptr != NULL; ptr = ptr->ai_next) {
        clientsock = socket(ptr->ai_family, ptr->ai_socktype, ptr->ai_protocol);
        if (clientsock == INVALID_SOCKET) {
            printf("could not create socket\n");
            freeaddrinfo(result);
            WSACleanup();
            return 1;
        }

        int iResult = connect(clientsock, ptr->ai_addr, (int)ptr->ai_addrlen);
        if (iResult == SOCKET_ERROR) {
            printf("error while connecting to server: %d\n", WSAGetLastError());
            freeaddrinfo(result);
            WSACleanup();
            return 1;
        }

    }
    printf("connected to server successfully\n");

    // takes username of the user 
    printf("USERNAME: ");
    signal(clientsock, 0x02); // signals server that new user is joining and to tells to get ready to revieve username
    sendtoserver(clientsock); // sends username to the server

    signal(clientsock, 0x01); // signals that user is online

    // takes username of the client to whome to chat with
    do {
        printf("Enter username to connect\n>>> "); // enter username of the client
        username = sendtoserver(clientsock); // sends client username to server to verify
        recv(clientsock, &recvbyte, 1, 0); // server sends verification signal 
        
        // 0x03 for client username is available 0x04 for not
        if ((int)recvbyte == 0x03) {
            printf("connecting to ", username.c_str(), "\n");
            break;
        }
        else {
            printf("Enter correct username\n");

        }
    } while (1);

    // runs the thread to send and recieve funtions to immitate chat
    std::thread recievechat(recvmsg , clientsock, username);
    std::thread sendchat(sendmsg, clientsock);
    
    // joins thread to main thread
    sendchat.join();
    recievechat.join();

    // fuking done
    closesocket(clientsock);
    WSACleanup();
	return 0;
}

