﻿using System.Net;
using MyFramework;
using MyFramework.Attributes;
using MyFramework.Contracts;
using Persistence.Entities;
using WebAPI.Models;
using WebAPI.Services;


namespace WebAPI.Controllers;

public class MainController(ArtworkService artworkService): MyBaseController
{
    [HttpGet("/")]
    public IMyActionResult ShowIndexAsync()
    {
        const string path = "public/MainPage/index.html";
        return new ResourceResult(path);
    }

    [HttpGet("/api/get-artworks")]
    public async Task<IMyActionResult> GetArtworksForGalleryAsync(CancellationToken cancellationToken)
    {
        var artworksResult = await artworkService.GetArtworksInfoAsync(cancellationToken);

        return artworksResult.IsSuccess
            ? new JsonResult<List<ResponseArtworkModel>>(artworksResult.Data)
            : new ErrorResult(artworksResult.StatusCode, artworksResult.ErrorMessage!);
    }
}