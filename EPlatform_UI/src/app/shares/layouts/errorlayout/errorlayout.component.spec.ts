import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ErrorlayoutComponent } from './errorlayout.component';

describe('ErrorlayoutComponent', () => {
  let component: ErrorlayoutComponent;
  let fixture: ComponentFixture<ErrorlayoutComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ErrorlayoutComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ErrorlayoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
