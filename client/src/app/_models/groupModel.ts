export interface GroupModel{
    name: string;
    connections: ConnectionModel[];
}

interface ConnectionModel{
    connectionId: string;
    username: string;
}