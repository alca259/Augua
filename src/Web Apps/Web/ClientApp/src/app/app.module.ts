import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { BlogModule } from './modules/blog/blog.module';
import { HomeModule } from './modules/home/home.module';
import { PortfolioModule } from './modules/portfolio/portfolio.module';
import { ProjectModule } from './modules/project/project.module';
import { NavBarComponent } from './shared/components/nav-bar/nav-bar.component';
import { NotFoundPageComponent } from './shared/pages/not-found-page/not-found-page.component';
import { UnauthorizedPageComponent } from './shared/pages/unauthorized-page/unauthorized-page.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RootComponentComponent } from './shared/components/root-component/root-component.component';

@NgModule({
  declarations: [
    NavBarComponent,
    NotFoundPageComponent,
    RootComponentComponent,
    UnauthorizedPageComponent,
  ],
  imports: [
    AppRoutingModule,
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,

    BlogModule,
    HomeModule,
    PortfolioModule,
    ProjectModule
  ],
  providers: [],
  bootstrap: [RootComponentComponent]
})
export class AppModule { }
