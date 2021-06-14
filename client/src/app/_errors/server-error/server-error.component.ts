import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-server-error',
  templateUrl: './server-error.component.html',
  styleUrls: ['./server-error.component.css']
})
export class ServerErrorComponent implements OnInit {

  error: any

  constructor(
    private router: Router
  ) { 
    // Não pode ser acessado no ngOnInit, somente no constructor
    const navigation =  this.router.getCurrentNavigation();
    // Caso o usuário de um refresh o navigation é perdido, logo é incerto
    this.error = navigation?.extras?.state?.error;
  }

  ngOnInit(): void {
  }

}
