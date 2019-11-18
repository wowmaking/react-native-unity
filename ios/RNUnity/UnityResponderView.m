//
//  UnityResponderView.m
//  RNUnity
//
//  Created by Глеб Волчецки on 8/30/19.
//  Copyright © 2019 Facebook. All rights reserved.
//

#import "UnityResponderView.h"

#import "RNUnity.h"


@implementation UnityResponderView

-(UIView *)hitTest:(CGPoint)point withEvent:(UIEvent *)event
{
    UIView *view = [[[RNUnity ufw] appController] rootView];
    [view hitTest:point withEvent:event];
    return view;
}

@end
