import { Component, Output, EventEmitter, OnInit, Input, OnChanges, SimpleChanges, AfterViewInit, Inject, inject } from '@angular/core';
import { PageModel } from '../../../components/models/PageModel';
import { last } from 'rxjs';
import { DOCUMENT } from '@angular/common';

@Component({
  selector: 'app-pagination',
  standalone: true,
  imports: [],
  templateUrl: './pagination.component.html',
  styleUrl: './pagination.component.scss'
})

export class PaginationComponent implements OnInit, OnChanges {
  ngOnChanges(changes: SimpleChanges): void {
    if (changes["totalItem"] || changes["limit"] || changes["pageIndex"]){
      this.totalPage = Math.ceil(this.totalItem/this.limit);
      this.loadPage();
    }
  }
  ngOnInit(): void {
    this.pageChange(1);
  }
  @Output() page = new EventEmitter<PageModel>();
  numbers: number[] = [];
  totalPage!: number;
  hasPrevious: boolean = false;
  hasNext: boolean = false;
  @Input() pageIndex: number = 1;
  @Input() limit: number = 10;
  @Input() totalItem: number = 1;
  document = inject(DOCUMENT);

  loadPage(){
    this.numbers = [];

    if (this.pageIndex > this.totalPage){
      this.pageIndex = this.totalPage;
    }

    if (this.pageIndex < 1){
      this.pageIndex = 1;
    }

    let begin = this.pageIndex - 4;
    let end = this.pageIndex + 5;

    // Begin is negative then add all page to end
    if (begin < 1){
      end = (end - begin) + 1;
      begin = 1;
    }

    if (end > this.totalPage){
      begin = this.totalPage - 9;
      end = this.totalPage;
    }

    if (this.totalPage < 10){
      begin = 1;
      end = this.totalPage;
    }
    
    this.hasNext = this.pageIndex == this.totalPage ? false : true;
    this.hasPrevious = this.pageIndex == 1 ? false : true;

    for (let i = begin; i <= end; i++){
      this.numbers.push(i);
    }

    this.document.querySelectorAll(".page-element").forEach(element => {
      if (element.textContent == this.pageIndex.toString()){
        element.classList.add('active');
      } else {
        element.classList.remove('active');
      }
    })
  }

  pageChange(pageIndex: number){
    this.pageIndex = pageIndex;
    let pageInfo: PageModel = {
      pageIndex: this.pageIndex,
      pageSize: this.limit
    }

    this.page.emit(pageInfo);
  }
}
