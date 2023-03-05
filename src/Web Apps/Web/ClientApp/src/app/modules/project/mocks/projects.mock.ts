import { IProject, Levels } from "../models/interfaces/Project.interface";

export const MOCK_PROJECTS: IProject[] = [
    {
        id: 1,
        title: 'Project 1',
        description: 'Description 1',
        completed: false,
        level: Levels.Info
    },
    {
        id: 2,
        title: 'Project 2',
        description: 'Description 2',
        completed: true,
        level: Levels.Urgent
    },
    {
        id: 3,
        title: 'Project 3',
        description: 'Description 3',
        completed: true,
        level: Levels.Info
    }
];