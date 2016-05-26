//
//  VideoConverter.h
//  VideoConverter
//
//  Created by Isaac Cheng on 11/26/14.
//  Copyright (c) 2015 Champ Info Co., Ltd. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <AssetsLibrary/AssetsLibrary.h>

extern "C" void InitConverter(const char* object, const char* method, const char* audioPath, int width, int height, int fps, bool shortestClip);
extern "C" void EncodeVideoData(const char* bytes, int bytesLength, int frameIndicator);
extern "C" void FinishedEncodingVideo();
extern "C" void DisplayAlertView(const char* message);

@interface VideoConverter : NSObject

@end