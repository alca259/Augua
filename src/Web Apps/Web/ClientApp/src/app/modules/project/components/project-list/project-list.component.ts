import { Component, OnInit } from '@angular/core';
import { IProject, Levels } from '../../models/interfaces/Project.interface';

@Component({
  selector: 'app-project-list',
  templateUrl: './project-list.component.html',
  styleUrls: ['./project-list.component.scss']
})
export class ProjectListComponent implements OnInit {

  proyecto1: IProject = {
    title: 'Project 1',
    description: 'Description 1',
    completed: false,
    level: Levels.Info
  };

  proyecto2: IProject = {
    title: 'Project 2',
    description: 'Description 2',
    completed: true,
    level: Levels.Urgent
  };

  constructor() {}

  ngOnInit(): void {

  }

  deleteProject(project: IProject){
    console.log(`Deleted project ${project.title}`);
  }

}
