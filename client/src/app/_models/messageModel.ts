export interface MessageModel {
    id: number;
    senderId: number;
    senderUsername: string;
    senderPhotoUrl: string;
    senderKnownA: string;
    recipientId: number;
    recipientUsername: string;
    recipientPhotoUrl: string;
    recipientKnownAs: string;
    content: string;
    dateRead?: Date;
    messageSent: Date;
}