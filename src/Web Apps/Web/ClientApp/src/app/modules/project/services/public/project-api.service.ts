import { Injectable } from '@angular/core';
import { MOCK_PROJECTS } from '../../mocks/projects.mock';
import { IProject } from '../../models/interfaces/Project.interface';

@Injectable({
  providedIn: 'root'
})
export class ProjectApiService {

  constructor() { }

  getAllProjects() : IProject[] {
    return MOCK_PROJECTS;
  }

  getProjectByID(id: number) : IProject | undefined {
    let projectFind = MOCK_PROJECTS.find((p : IProject) => p.id === id);
    if (projectFind) return projectFind;
    return;
  }
}
