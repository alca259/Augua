export enum Levels {
    "Info" = 0,
    "Urgent" = 1
}

export interface IProject {
    title: string;
    description?: string;
    completed: boolean;
    level: Levels;
}