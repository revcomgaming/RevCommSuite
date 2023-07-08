# RevCommSuite

RevCommSuite is an open source API that allows communications capabilities to be added to applications that can run scripts or access local files, giving them the ability for bi-directional communication. It added functionality includes:

- Connecting to a servers using TCP or web socket connections
- Creating Peer-to-Peer connections
- Database connections
- URL and web server communications
- File downloads
- Ability to map client and database information to each other
- Ability to group clients together for session based group communications
- Support for SSL and OpenSSL encryption
- Ability to create connections between multiple applications
- Seamless recovery of lost messages between client and server through replay functionality

2.3 Major Update:

- Added new processor for C#: RevCommProcessor.cs (with limited functionality)
- Improved RevCommServer data map and process functionality with new 'maxconnectionsperobject' setting for applying a number of max database connections.
- Added HTTP transmission functionality for RevCommProcessor (C#)
- Update to OpenSSL 3.0.8 and TLS 1.2
- Fixes for SSL usage
- Fixes for HTTP transmission functionality
