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
    return [RNUnity.ufw.appController.window hitTest:point withEvent:event];
}

@end
