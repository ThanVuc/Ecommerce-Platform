import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateRoleClaimComponent } from './update-role-claim.component';

describe('UpdateRoleClaimComponent', () => {
  let component: UpdateRoleClaimComponent;
  let fixture: ComponentFixture<UpdateRoleClaimComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UpdateRoleClaimComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UpdateRoleClaimComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
