import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShopownerlayoutComponent } from './shopownerlayout.component';

describe('ShopownerlayoutComponent', () => {
  let component: ShopownerlayoutComponent;
  let fixture: ComponentFixture<ShopownerlayoutComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ShopownerlayoutComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ShopownerlayoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
