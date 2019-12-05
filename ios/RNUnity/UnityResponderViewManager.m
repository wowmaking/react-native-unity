//
//  UnityResponderViewManager.m
//  RNUnity
//
//  Created by Глеб Волчецки on 8/30/19.
//  Copyright © 2019 Facebook. All rights reserved.
//
#import <React/RCTViewManager.h>
#import <UIKit/UIKit.h>

#import "UnityResponderViewManager.h"
#import "UnityResponderView.h"


@implementation UnityResponderViewManager

- (UIView *)view
{
    UnityResponderView *view = [[UnityResponderView alloc] initWithFrame:CGRectMake(0, 0, [[UIScreen mainScreen] bounds].size.width, [[UIScreen mainScreen] bounds].size.height)];
    return view;
}

RCT_EXPORT_MODULE(UnityResponderView)

@end
