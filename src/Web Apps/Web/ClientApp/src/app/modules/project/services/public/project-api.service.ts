import { Injectable } from '@angular/core';
import { MOCK_PROJECTS } from '../../mocks/projects.mock';
import { IProject } from '../../models/interfaces/Project.interface';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';


@Injectable({
  providedIn: 'root'
})
export class ProjectApiService {

  constructor(
    private clientHttp: HttpClient
  ) { }

  getAllProjects() : IProject[] {
    return MOCK_PROJECTS;
  }

  getProjectByID(id: number) : IProject | undefined {
    let projectFind = MOCK_PROJECTS.find((p : IProject) => p.id === id);
    if (projectFind) return projectFind;
    return;
  }

  getAllProjectsAsync() : Promise<IProject[]> {
    return Promise.resolve(MOCK_PROJECTS);
  }

  getProjectByIdAsync(id: number) : Promise<IProject> | undefined {
    this.clientHttp.post('https://localhost:5020/account/login', {})
      .subscribe((result) => {
        console.log("OK", result);
        sessionStorage.setItem('Token', result.token);
      },
      (error) => console.error(`Ha ocurrido algún problema con la petición. ${error}`));

    let projectFind = MOCK_PROJECTS.find((p: IProject) => p.id === id);
    if (projectFind) return Promise.resolve(projectFind);
    return;
  }

  getProjectByIdObservableAsync(id: number) : Observable<IProject> | undefined {
    let projectFind = MOCK_PROJECTS.find((p: IProject) => p.id === id);

    let obs = new Observable<IProject>(observer => {
      observer.next(projectFind); // Emite un valor a todo componente suscrito
      observer.complete(); // No emitimos más
    });

    if (projectFind) obs;
    return;
  }
}
