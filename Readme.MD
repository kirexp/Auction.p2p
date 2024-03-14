## Disclaimer
Please take into considiration the fact that I had a time frame and some of the tactics\approaches were implemented ASAP. 
Some Variables were named in a very ugly way.
Separation of Layers and Business\Domain units has not been done as well. 
Some places of code have not-necessary converts\casts like double to decimal, guid -> string -> guid and etc.

## Implemented
- Centralized service that holds information about client's ip-addresses. 
 (When Client starts the app. App is looking for a open port to be able to use it for handling p2p connections;) 
- CLI for client. (it supports a few commands like "list", "create-lot", "bid", "quit")
- Implemented GRPC communication using official nuget package. 
- Implemented cross-platform solution using .NET6. (development work was done and tested on MacOS-lates version)

## FYI: 
- CLI operation "create-lot" generates a gibberish data for title and etc. 
- Image part of the task was omitted. 

## Not implemented
- SQL-Lite (decided to go with in-memory due to the fact that SQL part of the task is the easiest part hence would require some time to set-up a boilerplate. 
- Auction-Conclusion step was not implemented because of time matter.


### Auction-conclusion step 
If I had time I would have implemented it according to either P2P Gossip approach or notifying each user who did bidding (we store that info along with bids). Everything would depend on requirements.


