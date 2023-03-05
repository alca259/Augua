import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { IProject } from '../../models/interfaces/Project.interface';

@Component({
  selector: 'tr [app-project]',
  templateUrl: './project.component.html',
  styleUrls: ['./project.component.scss']
})
export class ProjectComponent implements OnInit {

  @Input() project: IProject | undefined;
  @Output() deleteEvent: EventEmitter<IProject> = new EventEmitter<IProject>;

  constructor() {}

  ngOnInit(): void {
  }

  deleteProject() {
    this.deleteEvent.emit(this.project);
  }

}
